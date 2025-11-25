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
        eventTitle = Uri.EscapeDataString(eventTitle);
        string start = reservationDateTime.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
        string end = reservationDateTime.Add(visitDuration).ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
        string location = Uri.EscapeDataString(_museumLocation);

        _googleCalendarUrl = $"{_googleCalendarBaseUrl}&text={eventTitle}&dates={start}/{end}&location={location}";
    }

    public void OpenInCalendarApp()
    {
        Application.OpenURL(_googleCalendarUrl);
    }
}
