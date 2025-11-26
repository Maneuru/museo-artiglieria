using System;
using System.Collections.Generic;
using TMPro;
using UI.PageNavigation;
using UnityEngine;
using UnityEngine.UI;

public class VisualizerPage : Page
{
    [SerializeField] private Transform _modelContainer;
    [SerializeField] private Transform _sceneContainer;
    [SerializeField] private GameObject _notFoundMessage;
    [SerializeField] private VisualizableUIItem _assetListItemPrefab;

    [Header("Page References")]
    [SerializeField] private Transform _assetUIListContainer;
    [SerializeField] private TMP_Text _assetTitle;
    [SerializeField] private TMP_Text _assetDescription;

    private readonly Dictionary<int, Visualizable> _loadedModels = new();
    private readonly Dictionary<int, Button> _loadedModelsUI = new();

    private Visualizable _currentModel;
    private bool _awakened = false;

    private void Awake()
    {
        if (_awakened)
        {
            return;
        }

        _awakened = true;
        if (_modelContainer == null)
        {
            throw new NullReferenceException($"{nameof(_modelContainer)} is not assigned in the {nameof(VisualizerPage)}.");
        }

        foreach (var model in _modelContainer.GetComponentsInChildren<Visualizable>(true))
        {
            int key = model.assetID;
            _loadedModels[key] = model;
            model.gameObject.SetActive(false);

            VisualizableUIItem visualizableUI = Instantiate(_assetListItemPrefab, _assetUIListContainer);
            visualizableUI.Init(model.assetName, model.previewImage);

            Button button = visualizableUI.GetComponent<Button>();
            button.onClick.AddListener(() => ActivateVisualizer(key));

            _loadedModelsUI[key] = button;
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
        DeactivateVisualizer();
        _sceneContainer.gameObject.SetActive(true);

        if (_loadedModels.TryGetValue(assetID, out Visualizable model))
        {
            _notFoundMessage.SetActive(false);

            _currentModel = model;
            _currentModel.gameObject.SetActive(true);
            _loadedModelsUI[assetID].interactable = false;

            _assetTitle.text = _currentModel.assetName;
            _assetDescription.text = _currentModel.description;
        }
        else
        {
            _notFoundMessage.SetActive(true);
        }
    }

    public void DeactivateVisualizer()
    {
        _sceneContainer.gameObject.SetActive(false);

        if (_currentModel != null)
        {
            _currentModel.gameObject.SetActive(false);
            _loadedModelsUI[_currentModel.assetID].interactable = true;
            _currentModel = null;
        }
    }
}
