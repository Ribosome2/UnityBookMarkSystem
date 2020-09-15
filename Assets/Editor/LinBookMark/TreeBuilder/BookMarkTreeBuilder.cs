using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkTreeBuilder
    {
        public TreeViewItem BuildRoot( List<TreeViewItem> allItems )
        {
            var bookMarkDataModel = BookMarkDataCenter.instance.bookMarkDataModel;
            BookMarkDataCenter.instance.ExpandDataMgr.ClearExpandDataMap();
            var root =CreateTreeViewItemForBookMarkElement(bookMarkDataModel.root);
            List<TreeElement> elements = new List<TreeElement>();
            TreeElementUtility.TreeToList<TreeElement>(bookMarkDataModel.root,elements);
            for (int i = 1; i < elements.Count; i++)
            {
                var element = elements[i];
                var bookMarkElement = element as LinBookMarkElement;
                var treeItem = CreateTreeViewItemForBookMarkElement(bookMarkElement);
                // Debug.Log("re add "+treeItem.displayName + "depth"+ treeItem.depth);
                AddToTreeItemList(allItems, treeItem);

                if (bookMarkElement.type == BookMarkType.AssetFolder)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(bookMarkElement.AssetGuild);
                    AutoAddSubAssetToTree(allItems, assetPath, treeItem);
                }
            }

            return root;
        }

        private static void AddToTreeItemList(List<TreeViewItem> allItems, TreeViewItem treeItem)
        {
            if (allItems.Count > 0 && treeItem.depth < 0)
            {
                Debug.LogError("error node depth , I dont want you ! : "+treeItem.displayName);
                return;
            }
//            Debug.Log("add "+treeItem.id+treeItem.displayName);
            allItems.Add(treeItem);
        }

        private static void AutoAddSubAssetToTree(List<TreeViewItem> allItems, string assetPath, TreeViewItem treeItemParent)
        {
           
            if (!string.IsNullOrEmpty(assetPath) && Directory.Exists(assetPath))
            {
                var subDirectories = Directory.GetDirectories(assetPath);
                foreach (var subDirectory in subDirectories)
                {
                    var subItem = CreateFolderTreeItem(treeItemParent, subDirectory);
                    AddToTreeItemList(allItems, subItem);
                    AutoAddSubAssetToTree(allItems, subDirectory, subItem);
                }

                var subFiles = Directory.GetFiles(assetPath).Where(x => x.EndsWith(".meta")==false);
                foreach (var subFile in subFiles)
                {
                    var fileInfo = new FileInfo(subFile);
                    var subItem = CreateFileTreeItem(treeItemParent, fileInfo, subFile);
                    AddToTreeItemList(allItems, subItem);
                }
                
            }
        }

        private static TreeViewItem CreateFileTreeItem(TreeViewItem treeItemParent, FileInfo fileInfo, string subFile)
        {
            var treeItemId = TreeItemIdGenerator.NextId;
            TreeViewItem subItem = new TreeViewItem(treeItemId, treeItemParent.depth + 1, fileInfo.Name);
            var expandData = new ExpandData() {AssetPath = subFile};
            BookMarkDataCenter.instance.ExpandDataMgr.SetExpandData(treeItemId, expandData);
            subItem.icon = (Texture2D) AssetDatabase.GetCachedIcon(subFile);
            return subItem;
        }

        private static TreeViewItem CreateFolderTreeItem(TreeViewItem treeItemParent, string subDirectory)
        {
            var dirInfo = new DirectoryInfo(subDirectory);
            var treeItemId = TreeItemIdGenerator.NextId;
            TreeViewItem subItem = new TreeViewItem(treeItemId, treeItemParent.depth + 1, dirInfo.Name);

            var expandData = new ExpandData() {AssetPath = subDirectory};
            BookMarkDataCenter.instance.ExpandDataMgr.SetExpandData(treeItemId, expandData);
            subItem.icon = (Texture2D) AssetDatabase.GetCachedIcon(subDirectory);
            return subItem;
        }

        static TreeViewItem CreateTreeViewItemForBookMarkElement (LinBookMarkElement element)
        {
            var id = element.id;
            var item = new TreeViewItem(id, element.depth, element.name);
            if (element.type == BookMarkType.CustomRoot)
            {
                item.icon = element.GetIcon();
            }
            return item;
        }
        
    }
}