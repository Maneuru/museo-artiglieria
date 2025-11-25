using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class AssetDownloader : MonoBehaviour
{
    public const string assetDownloadBasePath = "Assets/DownloadedAssets/";
    public string assetName => $"{_filename}{_assetExtension}";
    public string assetPath => $"{Application.persistentDataPath}/{assetDownloadBasePath}/{assetName}";

    [Header("Asset Download Settings")]
    [SerializeField] private string _url = "";
    [SerializeField] private string _filename = "";
    [SerializeField] private string _assetExtension = ".json";

    [SerializeField]
    private bool _fetchOnAwake = true;
    [SerializeField, ShowCondition(nameof(_fetchOnAwake))]
    private bool _ignoreExpireTimeOnAwake = false;

    [SerializeField]
    private bool _hasExpireTime = false;
    [SerializeField, ShowCondition(nameof(_hasExpireTime))]
    private double _expireTime = 3600f;

    private void OnValidate()
    {
        if (!Regex.IsMatch(_url, "^http(s)?://"))
        {
            Debug.LogWarning("Invalid URL provided for asset download.");
        }

        if (!Regex.IsMatch(_assetExtension, "^\\.[a-zA-Z0-9]+$"))
        {
            _assetExtension = ".notvalidextension";
        }

        if (string.IsNullOrEmpty(_filename) || !Regex.IsMatch(_filename, "^[a-zA-Z0-9_\\-]+$"))
        {
            Debug.LogWarning("File name is required and should be an alphanumerical value. Accepted characters: '_-'");
        }
    }

    public void Awake()
    {
        if (string.IsNullOrEmpty(_filename) || !Regex.IsMatch(_filename, "^[a-zA-Z0-9_\\-]+$"))
        {
            throw new FormatException($"Invalid format for {nameof(_filename)}.");
        }

        if (_fetchOnAwake)
        {
            FetchAsset(_ignoreExpireTimeOnAwake);
        }
    }

    public void FetchAsset(bool ignoreExpireTime = false)
    {
        string dirPath = Path.Combine(Application.persistentDataPath, assetDownloadBasePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        FileInfo file = new(assetPath);

        GameManager gm = GameManager.GetInstanceOrCreate();

        if (!file.Exists)
        {
            gm.StartCoroutine(Request());
            return;
        }

        if (!_hasExpireTime || ignoreExpireTime)
        {
            return;
        }

        double timeSinceLastWrite = (System.DateTime.Now - file.LastWriteTime).TotalSeconds;
        if (timeSinceLastWrite > _expireTime)
        {
            gm.StartCoroutine(Request());
        }
    }

    private IEnumerator Request()
    {
        UnityWebRequest request = UnityWebRequest.Get(_url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download: " + request.error);
            yield break;
        }

        byte[] data = request.downloadHandler.data;
        File.WriteAllBytes(assetPath, data);
    }
}
