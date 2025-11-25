using System;
using UnityEngine;
using UnityEngine.Events;

public class DatePicker : MonoBehaviour
{

    [SerializeField] private UnityEvent<DateTime> _onDateSelected;

    private AndroidDatePicker _datePicker;

    private void Awake()
    {
        _datePicker = new AndroidDatePicker();
    }

    public void ShowDatePicker()
    {
        ShowDatePicker(DateTime.Now);
    }

    public void ShowDatePicker(DateTime initDate)
    {
        _datePicker.Show(initDate, _onDateSelected);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DatePicker))]
public class DatePickerEditor : UnityEditor.Editor
{
    private string _date;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _date = UnityEditor.EditorGUILayout.TextField("Test Date (yyyy-MM-dd)", _date);

        if (GUILayout.Button("Test Date Picker"))
        {
            if (!DateTime.TryParseExact(_date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return;
            }

            var fieldInfo = target.GetType().GetField("_onDateSelected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo.GetValue(target) is UnityEvent<DateTime> dateEvent)
            {
                dateEvent.Invoke(parsedDate);
            }
        }
    }
}
#endif

#if UNITY_ANDROID
public class AndroidDatePicker
{
    private UnityEvent<DateTime> _dateSelectedEvent;
    private DateTime _initDate;

    public void Show(DateTime initDate, UnityEvent<DateTime> callback)
    {
        _initDate = initDate;
        _dateSelectedEvent = callback;

        var unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread",
            new AndroidJavaRunnable(() =>
            {
                new AndroidJavaObject("android.app.DatePickerDialog", activity, new DateCallback(this),
                    _initDate.Year, _initDate.Month - 1, _initDate.Day).Call("show");
            })
        );
    }

    private void DateSelectedHandler(DateTime date)
    {
        _dateSelectedEvent?.Invoke(date);
    }

    class DateCallback : AndroidJavaProxy
    {
        private AndroidDatePicker _dialog;

        public DateCallback(AndroidDatePicker d) : base("android.app.DatePickerDialog$OnDateSetListener")
        {
            _dialog = d;
        }

#pragma warning disable
        private void onDateSet(AndroidJavaObject _, int year, int monthOfYear, int dayOfMonth)
        {
            var selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);

            _dialog.DateSelectedHandler(selectedDate);
        }
#pragma warning restore
    }
}
#endif
