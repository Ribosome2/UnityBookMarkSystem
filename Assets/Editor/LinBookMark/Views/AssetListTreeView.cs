using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LinBookMark
{
    public class AssetListTreeView:TreeView
    {
        public IList<string> pathList; 
        public AssetListTreeView(TreeViewState state) : base(state)
        {
        }

        public AssetListTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public void SetAssetPathList(IList<string> rootPaths)
        {
            pathList = rootPaths;
            BuildRoot();
            Reload();
        }
        
        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(0,-1,"assetRoot");
            var allItems = GetAssetItems();
            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }

        IList<TreeViewItem> GetAssetItems()
        {
            List<TreeViewItem> result = new List<TreeViewItem>();
            if (pathList != null)
            {
                foreach (var path in pathList)
                {
                    if (Directory.Exists(path))
                    {
                        var subFiles = Directory.GetFiles(path).Where(x => x.EndsWith(".meta")==false);
                        foreach (var subFile in subFiles)
                        {
                            var fileInfo = new FileInfo(subFile);
                            AddOneFileItem(subFile, result);
                        }
                    }
                    else
                    {
                        if (File.Exists(path))
                        {
                            AddOneFileItem(path, result);
                        }
                    }
                }
            }
            return result;
        }

        private static void AddOneFileItem(string filePath, List<TreeViewItem> result)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(filePath);
            if (obj)
            {
                TreeViewItem item = new TreeViewItem(obj.GetInstanceID(), 0, obj.name);
                item.icon = (Texture2D) AssetDatabase.GetCachedIcon(filePath);
                result.Add(item);
            }
        }
    }
}