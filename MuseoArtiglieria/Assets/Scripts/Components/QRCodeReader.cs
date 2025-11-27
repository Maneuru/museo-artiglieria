using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class QRCodeReader : MonoBehaviour
{
    private const string _apiEndpoint = "https://api.qrserver.com/v1/read-qr-code/";

    private static readonly Vector2Int _minResolution = new(128, 128);
    private static readonly Vector2Int _maxResolution = new(4096, 4096);

    [Header("Display Settings")]
    [SerializeField] private Vector2Int _textureResolution = new(512, 512);

    [Header("Performance")]
    [Min(0f), SerializeField] private float _waitForSeconds = .3f;
    [SerializeField] private UnityEvent<string> _onQRCodeDetected;

    private RawImage _rawImage;
    private Coroutine _readingCoroutine;
    private string _qrCodeResult;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _textureResolution.x = Mathf.Clamp(_textureResolution.x, _minResolution.x, _maxResolution.x);
        _textureResolution.y = Mathf.Clamp(_textureResolution.y, _minResolution.y, _maxResolution.y);
    }
#endif

    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        var tmp = new WebCamTexture(_textureResolution.x, _textureResolution.y);
        DestroyImmediate(tmp);
    }

    private void OnEnable()
    {
        _rawImage.texture = new WebCamTexture(_textureResolution.x, _textureResolution.y);
        StartReading();
    }

    private void OnDisable()
    {
        DestroyImmediate(_rawImage.texture);
        _rawImage.texture = null;
    }

    public void StartReading()
    {
        if (_readingCoroutine != null)
        {
            StopCoroutine(_readingCoroutine);
        }

        _readingCoroutine = StartCoroutine(ReadQRCode());
    }

    private IEnumerator ReadQRCode()
    {
        _qrCodeResult = "";
        WebCamTexture webCamTexture = (WebCamTexture)_rawImage.texture;

        webCamTexture.Play();
        yield return new WaitUntil(() => webCamTexture.width > 16 && webCamTexture.height > 16);

        Texture2D texture = new(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);

        while (string.IsNullOrEmpty(_qrCodeResult))
        {
            texture.SetPixels32(webCamTexture.GetPixels32());
            texture.Apply();

            WWWForm form = new();
            form.AddBinaryData("file", texture.EncodeToPNG(), "qr.png", "image/png");

            using (UnityWebRequest request = UnityWebRequest.Post(_apiEndpoint, form))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<QRCodeApiResponse>(request.downloadHandler.text[1..^1]);
                    if (response.symbol.Length > 0 && string.IsNullOrEmpty(response.symbol[0].error))
                    {
                        _qrCodeResult = response.symbol[0].data;
                        _onQRCodeDetected.Invoke(_qrCodeResult);
                    }
                }
            }

            yield return _waitForSeconds;
        }

        webCamTexture.Stop();
        _readingCoroutine = null;
    }
}

[System.Serializable]
public struct QRCodeApiResponse
{
    public string type;
    public Symbol[] symbol;

    [System.Serializable]
    public struct Symbol
    {
        public string data;
        public string error;
    }
}
