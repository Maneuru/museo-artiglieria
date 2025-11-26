using UnityEngine;

public class Visualizable : MonoBehaviour
{
    [SerializeField] private int _assetID;
    [SerializeField] private string _assetName;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _previewImage;

    public int assetID => _assetID;
    public string assetName => _assetName;
    public string description => _description;
    public Sprite previewImage => _previewImage;
}
