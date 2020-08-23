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

        public void OnGUI(Rect rect)
        {
            
            // if (CheckDrawSingleTexturePreview(rect)) return;
            CheckInit();
            m_TreeView.OnGUI(rect);
           
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


        public void SetAssetList(IList<string> folderList)
        {
            CheckInit();
            _assetPathList = folderList;
            m_TreeView.SetAssetPathList(folderList);
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
        }
        
        public string GetParentFolderDesc()
        {
            return m_TreeView == null ? string.Empty : m_TreeView.GetParentFolderDesc();
        }

    }
}