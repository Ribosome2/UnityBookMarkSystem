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
            var root =CreateTreeViewItemForBookMarkElement(bookMarkDataModel.root);
            List<TreeElement> elements = new List<TreeElement>();
            TreeElementUtility.TreeToList<TreeElement>(bookMarkDataModel.root,elements);
            for (int i = 1; i < elements.Count; i++)
            {
                var element = elements[i];
                var bookMarkElement = element as LinBookMarkElement;
                var treeItem = CreateTreeViewItemForBookMarkElement(bookMarkElement);
                // Debug.Log("re add "+treeItem.displayName + "depth"+ treeItem.depth);
                allItems.Add(treeItem);

                if (bookMarkElement.type == BookMarkType.AssetFolder)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(bookMarkElement.AssetGuild);
                    AutoAddSubAssetToTree(allItems, assetPath, treeItem);
                }
                
            }

            return root;

        }

        private static void AutoAddSubAssetToTree(List<TreeViewItem> allItems, string assetPath, TreeViewItem treeItemParent)
        {
           
            if (!string.IsNullOrEmpty(assetPath))
            {
                var subDirectories = Directory.GetDirectories(assetPath);
                foreach (var subDirectory in subDirectories)
                {
                    Debug.Log("sub Folder " + subDirectory);
                    var dirInfo = new DirectoryInfo(subDirectory);
                    TreeViewItem subItem = new TreeViewItem(TreeItemIdGenerator.NextId, treeItemParent.depth + 1, dirInfo.Name);
                    allItems.Add(subItem);
                    AutoAddSubAssetToTree(allItems, subDirectory, subItem);
                }

                var subFiles = Directory.GetFiles(assetPath).Where(x => x.EndsWith(".meta")==false);
                foreach (var subFile in subFiles)
                {
                    var fileInfo = new FileInfo(subFile);
                    TreeViewItem subItem = new TreeViewItem(TreeItemIdGenerator.NextId, treeItemParent.depth + 1, fileInfo.Name);
                    allItems.Add(subItem);
                }
                
            }
        }

        static TreeViewItem CreateTreeViewItemForBookMarkElement (LinBookMarkElement element)
        {
            return new TreeViewItem(element.id, element.depth, element.name);
        }
        
    }
}