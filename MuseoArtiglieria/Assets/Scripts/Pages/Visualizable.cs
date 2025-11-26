using UnityEngine;

public class Visualizable : MonoBehaviour
{
    [SerializeField] private int _assetID;
    public int assetID => _assetID;
}
