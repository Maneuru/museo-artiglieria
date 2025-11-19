using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


#pragma warning disable IDE0130
namespace UI.PageNavigation

{
    [CustomEditor(typeof(PageManager))]
    public class PageManagerEditor : Editor
    {
        private string _pageName = "";
        private PageOpenMode _openMode = PageOpenMode.Replace;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PageManager pageManager = (PageManager)target;

            GUILayout.Space(10);
            GUILayout.Label("Page Manager Debugging", EditorStyles.boldLabel);
            System.Reflection.FieldInfo field = typeof(PageManager).GetField("_pageHistory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var pageHistory = (Stack<Page>)field.GetValue(pageManager);
            if (pageHistory.TryPeek(out Page page))
            {
                GUILayout.Label($"Go back to {page.gameObject.name}");
            }
            else
            {
                GUILayout.Label("No pages in history");
            }

            if (GUILayout.Button("Back"))
            {
                pageManager.Back();
            }

            GUILayout.Space(10);

            _pageName = EditorGUILayout.TextField("Page Name", _pageName);
            _openMode = (PageOpenMode)EditorGUILayout.EnumPopup("Open Mode", _openMode);

            if (GUILayout.Button("Open Page"))
            {
                var pages = pageManager.GetComponentsInChildren<Page>(true).ToList();
                var pageToOpen = pages.FirstOrDefault(p => p.gameObject.name == _pageName);
                if (pageToOpen != default(Page))
                {
                    pageManager.OpenPage(pageToOpen, _openMode);
                }
            }
        }
    }
}
