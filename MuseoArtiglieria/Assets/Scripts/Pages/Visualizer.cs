using System;
using System.Collections.Generic;
using UI.PageNavigation;
using UnityEngine;

public class Visualizer : Page
{
    [SerializeField] private Transform _modelContainer;
    [SerializeField] private Transform _sceneContainer;
    [SerializeField] private GameObject _notFoundMessage;
    [SerializeField] private Transform _assetListContainer;

    private Dictionary<int, Visualizable> _loadedModels = new();
    private Visualizable _currentModel;
    private bool _awakened = false;

    private void Awake()
    {
        _awakened = true;
        if (_modelContainer == null)
        {
            throw new NullReferenceException($"{nameof(_modelContainer)} is not assigned in the {nameof(Visualizer)}.");
        }

        foreach (var model in _modelContainer.GetComponentsInChildren<Visualizable>(true))
        {
            int key = model.assetID;
            _loadedModels[key] = model;
            model.gameObject.SetActive(false);
        }
    }

    public bool HasAsset(int assetID)
    {
        if (!_awakened)
        {
            Awake();
        }

        return _loadedModels.ContainsKey(assetID);
    }

    public void ActivateVisualizer(int assetID)
    {
        _sceneContainer.gameObject.SetActive(true);

        if (_currentModel != null)
        {
            _currentModel = null;
            _currentModel.gameObject.SetActive(false);
        }

        bool assetFound = _loadedModels.TryGetValue(assetID, out Visualizable model);
        _notFoundMessage.SetActive(!assetFound);

        if (assetFound)
        {
            _currentModel = model;
            _currentModel.gameObject.SetActive(true);
        }
    }

    public void DeactivateVisualizer()
    {
        _sceneContainer.gameObject.SetActive(false);

        if (_currentModel != null)
        {
            _currentModel = null;
            _currentModel.gameObject.SetActive(false);
        }
    }
}
