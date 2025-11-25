using UnityEngine;

public class MyReservations : UI.PageNavigation.Page
{
    [SerializeField] private ReservationItem _reservationItemPrefab;
    [SerializeField] private Transform _reservationsContainer;
    private ReservationData[] _reservations;

    private void OnEnable()
    {
        if (SaveSystem.LoadData(out ReservationsData reservationsWrapper, "Reservations"))
        {
            _reservations = reservationsWrapper.reservations;
        }

        for (int i = 0; i < _reservations.Length; i++)
        {
            bool exists = i < _reservationsContainer.childCount;
            ReservationItem item = exists
                ? _reservationsContainer.GetChild(i).GetComponent<ReservationItem>()
                : Instantiate(_reservationItemPrefab, _reservationsContainer);

            item.Setup(_reservations[i]);
        }
    }
}
