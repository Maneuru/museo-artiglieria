using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable IDE0130
namespace UI.PageNavigation
{
    public class PageManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Overlay _overlayContainer;
        private ScrollRect _scrollRect;
        private float _lastBackTime;

        private readonly Stack<Page> _pageHistory = new();
        private Page _currentPage;
        public Page currentPage
        {
            get
            {
                if (_currentPage == null && _scrollRect.content.TryGetComponent(out Page page))
                {
                    _currentPage = page;
                }

                return _currentPage;
            }
            private set
            {
                var page = value;
                _currentPage = page;
                page.gameObject.SetActive(true);
                _scrollRect.content = page.transform as RectTransform;
            }
        }

        private void Awake()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();

            if (_scrollRect == null)
            {
                throw new MissingComponentException("PageManager requires a ScrollRect component in its children.");
            }

            if (_scrollRect.content == null || !_scrollRect.content.TryGetComponent(out Page page))
            {
                throw new MissingReferenceException("PageManager's ScrollRect requires a content with page Component.");
            }

            if (_overlayContainer == null)
            {
                throw new MissingReferenceException("PageManager requires a reference to an overlay container RectTransform.");
            }

            foreach (var childPage in _scrollRect.viewport.GetComponentsInChildren<Page>(true))
            {
                childPage.originalParent = _scrollRect.viewport;
                if (childPage != currentPage)
                {
                    childPage.Deactivate();
                }
            }

            currentPage = page;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.time - _lastBackTime < 0.5f)
                {
                    Debug.Log("PageManager: Double back detected, exiting application.");
                    Application.Quit();
                    return;
                }

                _lastBackTime = Time.time;
                Back();
            }
        }

        public void OpenPage(Page newPage, PageOpenMode mode)
        {
            switch (mode)
            {
                case PageOpenMode.Replace:
                    ReplacePage(newPage);
                    break;
                case PageOpenMode.Overlay:
                    OpenOverlay(newPage);
                    break;
                default:
                    Debug.LogError($"PageManager: Unsupported PageOpenMode {mode}");
                    break;
            }
        }

        public void ReplacePage(Page newPage)
        {
            if (newPage != currentPage)
            {
                _pageHistory.Push(currentPage);
            }

            currentPage.Deactivate();
            currentPage = newPage;
        }

        public void OpenOverlay(Page page)
        {
            _overlayContainer.Open();
            _overlayContainer.SetContent(page);
        }

        public bool Back()
        {
            if (_overlayContainer.isOpen)
            {
                CloseOverlay();
                return true;
            }

            if (!_pageHistory.TryPop(out Page previousPage))
            {
                Debug.Log("PageManager: No pages in history to go back to.");
                return false;
            }

            UndoReplacement(previousPage);
            return true;
        }

        private void UndoReplacement(Page pageToRestore)
        {
            currentPage.Deactivate();
            currentPage = pageToRestore;
        }

        private void CloseOverlay()
        {
            _overlayContainer.Close();
        }
    }

    public enum PageOpenMode
    {
        Replace,
        Overlay
    }
}
