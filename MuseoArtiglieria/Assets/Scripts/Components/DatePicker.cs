using System;
using UnityEngine;
using UnityEngine.Events;

public class DatePicker : MonoBehaviour
{

    [SerializeField] private UnityEvent<DateTime> _onDateSelected;

    private AndroidDatePicker _datePicker;
    private DateTime? _pendingDate;

    private void Awake()
    {
        _datePicker = new AndroidDatePicker();
    }

    private void Update()
    {
        if (_pendingDate.HasValue)
        {
            _onDateSelected?.Invoke(_pendingDate.Value);
            _pendingDate = null;
        }
    }

    private void InvokeUnityEvent(DateTime date)
    {
        _pendingDate = date;
    }

    public void ShowDatePicker()
    {
        ShowDatePicker(DateTime.Now);
    }

    public void ShowDatePicker(DateTime initDate)
    {
        _datePicker.Show(initDate, InvokeUnityEvent);
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
    private Action<DateTime> _dateSelectedCallback;
    private DateTime _initDate;

    public void Show(DateTime initDate, Action<DateTime> callback)
    {
        _initDate = initDate;
        _dateSelectedCallback = callback;

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
        _dateSelectedCallback?.Invoke(date);
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
