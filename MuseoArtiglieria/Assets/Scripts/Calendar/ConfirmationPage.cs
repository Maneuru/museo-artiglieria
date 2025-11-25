using System;
using UI.PageNavigation;
using UnityEngine;

public class ConfirmationPage : Page
{
    private const string _googleCalendarBaseUrl = "https://calendar.google.com/calendar/render?action=TEMPLATE";
    private const string _museumLocation = "Mastio della Cittadella, C.so Galileo Ferraris, 2, 10121 Torino TO, Italy";
    private string _googleCalendarUrl;

    public void OnReservationConfirmed(DateTime reservationDateTime, TimeSpan visitDuration, string eventTitle = "Visit to Museo dell'Artiglieria")
    {
        string escapedEventTitle = Uri.EscapeDataString(eventTitle);
        string start = reservationDateTime.ToUniversalTime().ToISOString();
        string end = reservationDateTime.Add(visitDuration).ToUniversalTime().ToISOString();
        string location = Uri.EscapeDataString(_museumLocation);

        _googleCalendarUrl = $"{_googleCalendarBaseUrl}&text={escapedEventTitle}&dates={start}/{end}&location={location}";

        if (!SaveSystem.LoadData(out ReservationsData reservationsWrapper, "Reservations"))
        {
            reservationsWrapper = new ReservationsData(1);
        }
        else
        {
            Array.Resize(ref reservationsWrapper.reservations, reservationsWrapper.reservations.Length + 1);
        }

        reservationsWrapper.reservations[^1] = new(eventTitle, reservationDateTime);

        SaveSystem.SaveData(reservationsWrapper, "Reservations");

        string a = "";
        foreach (var reservation in reservationsWrapper.reservations)
        {
            a += $"{reservation.eventTitle} at {reservation.reservationDateTime}\n";
        }
        Debug.Log(a);
    }

    public void OpenInCalendarApp()
    {
        Application.OpenURL(_googleCalendarUrl);
    }
}

[Serializable]
public class ReservationData
{
    public string eventTitle;
    public string reservationDateTime;
    public ReservationData(string eventTitle, DateTime reservationDateTime)
    {
        this.eventTitle = eventTitle;
        this.reservationDateTime = reservationDateTime.ToUniversalTime().ToISOString();
    }
}

[Serializable]
public class ReservationsData
{
    public ReservationData[] reservations;

    public ReservationsData(int size = 0)
    {
        reservations = new ReservationData[size];
    }
}
