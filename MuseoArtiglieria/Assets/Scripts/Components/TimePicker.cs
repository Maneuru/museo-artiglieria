using System;
using UnityEngine;
using UnityEngine.Events;

public class TimePicker : MonoBehaviour
{
    [SerializeField] private UnityEvent<TimeSpan> _onTimeSelected;

    private AndroidTimePicker _timePicker;

    private void Awake()
    {
        _timePicker = new AndroidTimePicker();
    }

    public void ShowTimePicker()
    {
        _timePicker.Show(TimeSpan.Zero, _onTimeSelected);
    }

    public void ShowTimePicker(TimeSpan initTime)
    {
        _timePicker.Show(initTime, _onTimeSelected);
    }
}

#if UNITY_ANDROID
public class AndroidTimePicker
{
    private UnityEvent<TimeSpan> _onTimeSelected;
    private TimeSpan _initTime;

    public void Show(TimeSpan initTime, UnityEvent<TimeSpan> callback)
    {
        _initTime = initTime;
        _onTimeSelected = callback;

        var unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread",
            new AndroidJavaRunnable(() =>
            {
                new AndroidJavaObject("android.app.TimePickerDialog",
                    activity,
                    new TimeCallback(this),
                    _initTime.Hours,
                    _initTime.Minutes,
                    true // is24HourView
                ).Call("show");
            }));
    }

    private void TimeSelectedHandler(TimeSpan time)
    {
        _onTimeSelected?.Invoke(time);
    }

    class TimeCallback : AndroidJavaProxy
    {
        private AndroidTimePicker _dialog;

        public TimeCallback(AndroidTimePicker d) : base("android.app.TimePickerDialog$OnTimeSetListener")
        {
            _dialog = d;
        }

#pragma warning disable
        private void onTimeSet(AndroidJavaObject _, int hourOfDay, int minute)
        {
            var selectedTime = new TimeSpan(hourOfDay, minute, 0);
            _dialog.TimeSelectedHandler(selectedTime);
        }
#pragma warning restore
    }
}
#endif
