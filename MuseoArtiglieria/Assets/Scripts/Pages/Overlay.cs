using UnityEngine;
using UnityEngine.UI;

#pragma warning disable IDE0130
namespace UI.PageNavigation

{
    public class Overlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _containerRect;
        [SerializeField] private RectTransform _contentRect;

        public Page currentPage { get; private set; }

        public bool isOpen => _containerRect.gameObject.activeSelf;
        public bool hasContent => currentPage != null;

        private void OnValidate() => CheckReferences();
        private void Awake()
        {
            CheckReferences();
            _containerRect.gameObject.SetActive(false);
        }

        private void CheckReferences()
        {
            if (_containerRect == null)
            {
                throw new MissingReferenceException($"{nameof(Overlay)} requires a reference to its container RectTransform.");
            }
            else if (_containerRect.gameObject == gameObject)
            {
                throw new MissingReferenceException(
                    $"{nameof(Overlay)}'s container RectTransform cannot be in the same GameObject as the {nameof(Overlay)}."
                );
            }

            if (_contentRect == null)
            {
                throw new MissingReferenceException($"{nameof(Overlay)} requires a reference to its content RectTransform.");
            }
        }

        /// <summary>
        /// Opens the overlay container if it is not already open.
        /// </summary>
        public void Open()
        {
            if (isOpen)
            {
                Debug.LogWarning($"{nameof(Overlay)} is already open. Use {nameof(Close)}() before opening a new one.");
                return;
            }

            _containerRect.gameObject.SetActive(true);
        }

        /// <summary>
        /// Sets the content of the overlay container to the specified page.
        /// </summary>
        /// <param name="page">
        /// The page to set as the content of the overlay container.
        /// </param>
        public void SetContent(Page page)
        {
            if (!isOpen)
            {
                Debug.LogWarning($"{nameof(Overlay)} is not open. Use {nameof(Open)}() first");
                return;
            }

            if (hasContent)
            {
                currentPage.Deactivate();
            }

            _scrollRect.content = page.rectTransform;
            page.UpdateParent(_contentRect);
            page.gameObject.SetActive(true);
            currentPage = page;
        }

        /// <summary>
        /// Closes the overlay container if it is currently open.
        /// Deactivates the container's GameObject and the current page, then clears the current page reference.
        /// Logs warnings if the container is already closed or if it is being closed without any content set.
        /// </summary>
        public void Close()
        {
            if (!isOpen)
            {
                Debug.LogWarning($"{nameof(Overlay)} is already closed.");
                return;
            }

            if (!hasContent)
            {
                Debug.LogWarning($"You are closing the {nameof(Overlay)} without any content set. Ensure to use use overlay correctly.");
            }
            else
            {
                currentPage.Deactivate();
                currentPage.BackToOriginalParent();
                currentPage = null;
            }

            _scrollRect.content = null;
            _containerRect.gameObject.SetActive(false);
        }
    }
}
