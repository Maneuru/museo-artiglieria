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
    [Min(0f), SerializeField] private float _waitForSeconds = 1f;
    [SerializeField] private UnityEvent<string> _onQRCodeDetected;

    private RawImage _rawImage;
    private Coroutine _readingCoroutine;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _textureResolution.x = Mathf.Clamp(_textureResolution.x, _minResolution.x, _maxResolution.x);
        _textureResolution.y = Mathf.Clamp(_textureResolution.y, _minResolution.y, _maxResolution.y);
    }
#endif

    private WebCamTexture _webCamTexture;
    private string _qrCodeResult;

    private void Start()
    {
        _webCamTexture = new WebCamTexture(_textureResolution.x, _textureResolution.y);
        _rawImage = GetComponent<RawImage>();
        _rawImage.texture = _webCamTexture;

        _webCamTexture.Play();
        _webCamTexture.Stop();

        StartReading();
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
        Texture2D texture = new(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
        _qrCodeResult = "";

        _webCamTexture.Play();

        while (string.IsNullOrEmpty(_qrCodeResult))
        {
            texture.SetPixels32(_webCamTexture.GetPixels32());
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

        _webCamTexture.Stop();
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
