using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
namespace LinBookMark
{
    public class LinBookMarkWindow : EditorWindow
    {
        [MenuItem("Tools/LinBookMark")]
        private static void ShowWindow()
        {
            var window = GetWindow<LinBookMarkWindow>();
            window.titleContent = new GUIContent("LinMark");
            window.Show();
        }

        // We are using SerializeField here to make sure view state is written to the window 
        // layout file. This means that the state survives restarting Unity as long as the window
        // is not closed. If omitting the attribute then the state just survives assembly reloading 
        // (i.e. it still gets serialized/deserialized)
        [SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        LinBookMarkTreeView m_TreeView;
        SearchField m_SearchField;

        void OnEnable ()
        {
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState ();

            m_TreeView = new LinBookMarkTreeView(m_TreeViewState);
            m_SearchField = new SearchField ();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            Debug.Log("view "+m_TreeView);
        }

        void OnGUI ()
        {
            DoToolbar ();
            DoTreeView ();
        }

        void DoToolbar()
        {
            GUILayout.BeginHorizontal (EditorStyles.toolbar);
            GUILayout.Space (100);
            GUILayout.FlexibleSpace();
            if (m_TreeView != null)
            {
                 m_TreeView.searchString = m_SearchField.OnToolbarGUI (m_TreeView.searchString);
            }
            GUILayout.EndHorizontal();
        }

        void DoTreeView()
        {
            if (m_TreeView != null)
            {
                Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                m_TreeView.OnGUI(rect);
            }
        }
    }
}