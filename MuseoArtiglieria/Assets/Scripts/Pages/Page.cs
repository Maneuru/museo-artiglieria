using UnityEngine;

#pragma warning disable IDE0130
namespace UI.PageNavigation
{
    public class Page : MonoBehaviour
    {
        private RectTransform _originalParent;
        public RectTransform rectTransform => transform as RectTransform;

        private void Awake() => _originalParent = transform.parent as RectTransform;

        public void UpdateParent(RectTransform newParent)
        {
            _originalParent ??= transform.parent as RectTransform;

            var height = rectTransform.rect.height;
            transform.SetParent(newParent);

            rectTransform.anchorMin = new(0, 1);
            rectTransform.anchorMax = new(1, 1);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new(0, height);
        }

        public void BackToOriginalParent()
        {
            if (_originalParent)
            {
                var height = rectTransform.rect.height;
                transform.SetParent(rectTransform);

                // rectTransform.anchorMin = new(0, 1);
                // rectTransform.anchorMax = new(1, 1);
                // rectTransform.anchoredPosition = new Vector2(0, 0);
                // rectTransform.sizeDelta = new(0, height);
                // rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Resets the Page to its original configuration and Disables it.
        /// </summary>
        public void Deactivate()
        {
            Reset();
            gameObject.SetActive(false);
        }

        private void Reset()
        {
            transform.SetParent(_originalParent);
            // TODO
        }
    }

}
