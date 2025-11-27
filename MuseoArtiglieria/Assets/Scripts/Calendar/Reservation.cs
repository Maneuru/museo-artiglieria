using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reservation : UI.PageNavigation.Page
{
    const string _dateFormat = "ddd dd MMM yyyy";
    const string _timeFormat = "hh:mm";

    [Header("Opening Hours Settings")]
    [SerializeField] private List<DailyOpeningHours> _dailyOpeningHours = new();

    [Header("Navigation")]
    [SerializeField] private UI.PageNavigation.PageManager _pageManager;
    [SerializeField] private ConfirmationPage _confirmationPage;

    [Header("References")]
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private Button _submitButton;

    [SerializeField] private TMP_Text _dateErrorText;
    [SerializeField] private TMP_Text _timeErrorText;

    private DateTime _selectedDate;
    private TimeSpan _selectedTime;
    private int _selectedDayOfWeekIndex = -1;
    public bool isValid => _submitButton.interactable;

    private void Awake()
    {
        _dateText.text = "-";
        _timeText.text = "-";
        _submitButton.interactable = false;
    }

    public void UpdateDate(DateTime date)
    {
        if (date == null)
        {
            _dateErrorText.gameObject.SetActive(true);
            return;
        }

        int index = _dailyOpeningHours.FindIndex(d => d.day == date.DayOfWeek);
        if (index == -1)
        {
            _dateErrorText.gameObject.SetActive(true);
            return;
        }

        _selectedDate = date;
        _selectedDayOfWeekIndex = index;
        _dateText.text = _selectedDate.ToString(_dateFormat).ToLowerInvariant();

        _dateErrorText.gameObject.SetActive(false);

        UpdateTime(_selectedTime);
        UpdateValidity();
    }

    public void UpdateTime(TimeSpan time)
    {
        Debug.Log("Updating time");
        if (time == null || _selectedDayOfWeekIndex == -1)
        {
            Debug.Log("Invalid time or day of week index");
            _selectedTime = TimeSpan.Zero;
            _timeErrorText.gameObject.SetActive(true);
            return;
        }

        DailyOpeningHours openingDay = _dailyOpeningHours[_selectedDayOfWeekIndex];

        if (!openingDay.IsOpenAt(time))
        {
            Debug.Log("Time not within opening hours, adjusting to first available time");
            time = openingDay.firstAvailableTime;
            if (!openingDay.IsOpenAt(time))
            {
                Debug.Log("Adjusted time still not within opening hours, showing error");
                _timeErrorText.gameObject.SetActive(true);
                return;
            }
        }

        Debug.Log("Time is valid, updating selection");
        _selectedTime = time;
        Debug.Log("Selected time: " + _selectedTime.ToString());
        _timeText.text = DateTime.MinValue.Add(_selectedTime).ToString(_timeFormat).ToLowerInvariant();

        _timeErrorText.gameObject.SetActive(false);

        UpdateValidity();
    }

    private void UpdateValidity()
    {
        _submitButton.interactable = _selectedDate != null
            && _selectedTime != null
            && _selectedDayOfWeekIndex != -1
            && _dailyOpeningHours[_selectedDayOfWeekIndex].IsOpenAt(_selectedTime);
    }

    public void OnSubmit()
    {
        if (!isValid)
        {
            return;
        }

        _pageManager.OpenPage(_confirmationPage, UI.PageNavigation.PageOpenMode.Overlay);
        _confirmationPage.OnReservationConfirmed(
            new DateTime(
                _selectedDate.Year,
                _selectedDate.Month,
                _selectedDate.Day,
                _selectedTime.Hours,
                _selectedTime.Minutes,
                0
            ),
            DailyOpeningHours.visitDuration
        );
    }
}
