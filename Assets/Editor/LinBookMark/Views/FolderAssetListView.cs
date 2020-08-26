using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LinBookMark
{
    [System.Serializable]
    public class FolderAssetListView
    {
        [SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        AssetListTreeView m_TreeView;
        private IList<string> _assetPathList;
        SearchField m_SearchField;
        private int toolBarHeight = 18;
        private Rect _drawRect;
        int treeViewYOffset =40;
        private AssetListGridView _gridView =new AssetListGridView();
        IList<string> _assetFileList = new List<string>();
        public void OnGUI(Rect rect,float iconSize )
        {
            _drawRect = rect;
            GUI.BeginClip(new Rect(rect.x,rect.y-15,rect.width,rect.height));
            // if (CheckDrawSingleTexturePreview(rect)) return;
            CheckInit();
            DoToolbar();
            if (iconSize > 16)
            {
                _gridView.OnGUI(_drawRect,_assetFileList,(int)iconSize);
            }
            else
            {
                m_TreeView.OnGUI(new Rect(0-10,treeViewYOffset,rect.width,rect.height-treeViewYOffset+15));
            }
            GUI.EndClip();
        }

        private bool CheckDrawSingleTexturePreview(Rect rect)
        {
            if (_assetPathList.Count == 1)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(_assetPathList[0]);
                var representation = AssetPreview.GetAssetPreview(asset);
                if (representation != null)
                {
                    var width = Mathf.Min(rect.width, representation.width);
                    var height = Mathf.Min(rect.height, representation.height);
                    GUI.DrawTexture(new Rect(rect.x, rect.y, width, height), representation);
                    return true;
                }
            }

            return false;
        }


        public void SetAssetList(IList<string> assetList)
        {
            CheckInit();
            _assetPathList = assetList;
            m_TreeView.SetAssetPathList(assetList);
            _assetFileList = FileUtil.GetAssetFileList(assetList);
        }

        public void RefreshAssetList()
        {
            if (m_TreeView != null)
            {
                m_TreeView.Reload();
            }
        }
      
        void CheckInit()
        {
            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
            }
            
            if (m_TreeView == null)
            {
                m_TreeView = new AssetListTreeView(m_TreeViewState);
            }
            if (m_SearchField == null)
            {
                m_SearchField = new SearchField();
            }
        }
        
        void DoToolbar()
        {
            if (m_TreeView != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Space(10);
                m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        
        public string GetParentFolderDesc()
        {
            return m_TreeView == null ? string.Empty : m_TreeView.GetParentFolderDesc();
        }

    }
}