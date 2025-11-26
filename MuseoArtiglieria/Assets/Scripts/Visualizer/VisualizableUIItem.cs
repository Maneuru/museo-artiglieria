using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualizableUIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _assetNameText;
    [SerializeField] private Image _assetImagePreview;

    public void Init(string assetName, Sprite assetPreview)
    {
        _assetNameText.text = assetName;
        _assetImagePreview.sprite = assetPreview;
    }
}
