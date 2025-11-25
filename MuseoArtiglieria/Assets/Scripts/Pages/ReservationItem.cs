using System;
using TMPro;
using UnityEngine;

public class ReservationItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _eventTitleText;
    [SerializeField] private TMP_Text _reservationDateText;
    [SerializeField] private TMP_Text _reservationTimeText;

    public void Setup(ReservationData data)
    {
        _eventTitleText.text = data.eventTitle;
        DateTime dateTime = DateTimeExtensions.FromISOString(data.reservationDateTime).ToLocalTime();
        _reservationDateText.text = dateTime.ToString("dddd, dd MMMM yyyy");
        _reservationTimeText.text = dateTime.ToString("HH:mm");
    }
}
