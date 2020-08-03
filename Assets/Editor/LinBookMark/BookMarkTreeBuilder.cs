using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkTreeBuilder
    {
        public TreeViewItem BuildRoot(TreeModel<LinBookMarkElement> bookMarkDataModel, List<TreeViewItem> allItems )
        {
            var root =CreateTreeViewItemForBookMarkElement(bookMarkDataModel.root);
            List<TreeElement> elements = new List<TreeElement>();
            TreeElementUtility.TreeToList<TreeElement>(bookMarkDataModel.root,elements);
            for (int i = 1; i < elements.Count; i++)
            {
                var element = elements[i];
                var bookMarkElement = element as LinBookMarkElement;
                var treeItem = CreateTreeViewItemForBookMarkElement(bookMarkElement);
                Debug.Log("re add "+treeItem.displayName + "depth"+ treeItem.depth);
                allItems.Add(treeItem);
            }

            return root;

        }
        
        static TreeViewItem CreateTreeViewItemForBookMarkElement (LinBookMarkElement element)
        {
            return new TreeViewItem(element.id, element.depth, element.name);
        }
        
    }
}