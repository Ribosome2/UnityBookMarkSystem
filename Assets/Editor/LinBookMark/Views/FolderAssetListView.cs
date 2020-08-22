using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LinBookMark
{
    public class FolderAssetListView
    {
        [SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        AssetListTreeView m_TreeView;

        public void OnGUI(Rect rect)
        {
            CheckInit();
            m_TreeView.OnGUI(rect);
        }


        public void SetAssetList(IList<string> folderList)
        {
            CheckInit();
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
    }
}