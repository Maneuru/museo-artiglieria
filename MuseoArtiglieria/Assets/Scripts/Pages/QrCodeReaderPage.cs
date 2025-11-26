using TMPro;
using UI.PageNavigation;
using UnityEngine;
using UnityEngine.UI;

public class QrCodeReaderPage : Page
{
    [SerializeField] private QRCodeReader _qrCodeReader;
    [SerializeField] private TMP_Text _errorText;

    [SerializeField] private PageManager _pageManager;
    [SerializeField] private VisualizerPage _visualizer;
    [SerializeField] private Button _goButton;

    private int _assetID;

    public void TryOpenVisualizer(string assetID)
    {
        bool shouldOpen = int.TryParse(assetID, out int id) && _visualizer.HasAsset(id);

        _errorText.gameObject.SetActive(!shouldOpen);
        _goButton.interactable = shouldOpen;

        if (shouldOpen)
        {
            _assetID = id;
        }
        else
        {
            _qrCodeReader.StartReading();
        }
    }

    public void GoToVisualizer()
    {
        _pageManager.OpenPage(_visualizer, PageOpenMode.Replace);
        _visualizer.ActivateVisualizer(_assetID);
    }
}
