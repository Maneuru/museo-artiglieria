using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VisualizerViewport : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private UnityEvent<Vector2> _onViewportDrag;
    [SerializeField] private UnityEvent<float> _onViewportPinch;

    private bool _isZooming => _secondaryTouch != null;

    private TouchInfo _primaryTouch;
    private TouchInfo _secondaryTouch;
    private float _lastZoomDistance;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_primaryTouch == null)
        {
            _primaryTouch = new TouchInfo(eventData.pointerId, eventData.position);
        }
        else if (_secondaryTouch == null && eventData.pointerId != _primaryTouch.fingerId)
        {
            _secondaryTouch = new TouchInfo(eventData.pointerId, eventData.position);
            _lastZoomDistance = Vector2.Distance(_primaryTouch.lastPosition, _secondaryTouch.lastPosition);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == _primaryTouch?.fingerId)
        {
            _primaryTouch.UpdatePosition(eventData.position);
        }

        if (_isZooming)
        {
            if (eventData.pointerId == _secondaryTouch.fingerId)
            {
                _secondaryTouch.UpdatePosition(eventData.position);
            }

            float currentZoomDistance = Vector2.Distance(_primaryTouch.lastPosition, _secondaryTouch.lastPosition);
            _onViewportPinch.Invoke(currentZoomDistance - _lastZoomDistance);
            _lastZoomDistance = currentZoomDistance;
        }
        else
        {
            _onViewportDrag.Invoke(eventData.delta);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == _primaryTouch?.fingerId)
        {
            _primaryTouch = null;
            if (_secondaryTouch != null)
            {
                _primaryTouch = _secondaryTouch;
                _secondaryTouch = null;
            }
        }
        else if (eventData.pointerId == _secondaryTouch?.fingerId)
        {
            _secondaryTouch = null;
        }
    }
}

public class TouchInfo
{
    public int fingerId;
    public Vector2 lastPosition;

    public TouchInfo(int fingerId, Vector2 lastPosition)
    {
        this.fingerId = fingerId;
        this.lastPosition = lastPosition;
    }

    public void UpdatePosition(Vector2 newPosition)
    {
        lastPosition = newPosition;
    }
}
