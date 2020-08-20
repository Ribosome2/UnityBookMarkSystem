using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkGUIStyles
    {
       
        public GUIStyle bottomBarBg = (GUIStyle) "ProjectBrowserBottomBarBg";
        public GUIStyle topBarBg = (GUIStyle) "ProjectBrowserTopBarBg";
        public GUIStyle selectedPathLabel = (GUIStyle) "Label";
        // public GUIStyle exposablePopup = ProjectBrowser.Styles.GetStyle("ExposablePopupMenu");
        public GUIStyle lockButton = (GUIStyle) "IN LockButton";
        public GUIStyle foldout = (GUIStyle) "AC RightArrow";
        public GUIContent m_FilterByLabel = new GUIContent((Texture) EditorGUIUtility.FindTexture("FilterByLabel"), "Search by Label");
        public GUIContent m_FilterByType = new GUIContent((Texture) EditorGUIUtility.FindTexture("FilterByType"), "Search by Type");
        public GUIContent m_CreateDropdownContent = new GUIContent("Create");
        public GUIContent m_SaveFilterContent = new GUIContent((Texture) EditorGUIUtility.FindTexture("Favorite"), "Save search");
        public GUIContent m_EmptyFolderText = new GUIContent("This folder is empty");
        public GUIContent m_SearchIn = new GUIContent("Search:");

        private static GUIStyle GetStyle(string styleName)
        {
            return (GUIStyle) styleName;
        }
    }
}