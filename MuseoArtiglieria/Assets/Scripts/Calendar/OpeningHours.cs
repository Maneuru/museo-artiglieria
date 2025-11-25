using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CustomTimeSpan
{
    private const int _minutesInterval = 15;

    [Min(0), SerializeField] private int _hours;
    [Min(0), SerializeField] private int _minutes;

    public static implicit operator TimeSpan(CustomTimeSpan cts)
    {
        int roundedMinutes = Mathf.RoundToInt(cts._minutes / _minutesInterval) * _minutesInterval;
        return new TimeSpan(cts._hours, roundedMinutes, 0);
    }
}

[Serializable]
public struct OpeningHours
{
    public CustomTimeSpan openTime;
    public CustomTimeSpan closeTime;
}

[Serializable]
public struct DailyOpeningHours
{
    public DayOfWeek day;
    public List<OpeningHours> hours;
    public static readonly TimeSpan visitDuration = new(1, 0, 0);

    public readonly bool hasOpeningHours => hours != null && hours.Count > 0;
    public readonly TimeSpan firstAvailableTime => hasOpeningHours ? hours[0].openTime : TimeSpan.Zero;

    public readonly bool IsOpen()
    {
        return IsOpenAt(DateTime.Now.TimeOfDay);
    }

    public readonly bool IsOpenAt(DateTime dateTime)
    {
        return IsOpenAt(dateTime.TimeOfDay);
    }

    public readonly bool IsOpenAt(TimeSpan currentTime)
    {
        if (!hasOpeningHours)
        {
            return false;
        }

        foreach (var openingHour in hours)
        {
            if (currentTime >= openingHour.openTime && currentTime < openingHour.closeTime - visitDuration)
            {
                return true;
            }
        }

        return false;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(CustomTimeSpan))]
public class CustomTimeSpanDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {

        var hoursProp = property.FindPropertyRelative("_hours");
        var minutesProp = property.FindPropertyRelative("_minutes");

        var halfWidth = position.width / 2;
        var height = UnityEditor.EditorGUIUtility.singleLineHeight;

        var labelRect = new Rect(position.x, position.y, position.width, height);
        var hoursRect = new Rect(position.x, position.y + height, halfWidth - 2, position.height);
        var minutesRect = new Rect(position.x + halfWidth + 2, position.y + height, halfWidth - 2, position.height);

        UnityEditor.EditorGUILayout.BeginVertical();
        UnityEditor.EditorGUILayout.Space(height);
        UnityEditor.EditorGUI.LabelField(labelRect, label, UnityEditor.EditorStyles.boldLabel);
        UnityEditor.EditorGUI.PropertyField(hoursRect, hoursProp, GUIContent.none);
        UnityEditor.EditorGUI.PropertyField(minutesRect, minutesProp, GUIContent.none);
        UnityEditor.EditorGUILayout.EndVertical();
    }
}
#endif
