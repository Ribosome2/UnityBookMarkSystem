using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LinBookMark
{
    public class LinBookMarkTreeView : TreeView
    {
       
        BookMarkTreeBuilder _treeBuilder = new BookMarkTreeBuilder();
        BookMarkDragDropHandler _dragDropHandler = new BookMarkDragDropHandler();

        public LinBookMarkTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            BookMarkDataCenter.instance.Init();
            Reload();
        }


        protected override TreeViewItem BuildRoot()
        {
            // BuildRoot is called every time Reload is called to ensure
            List<TreeViewItem> allItems = new List<TreeViewItem>();
            var root = _treeBuilder.BuildRoot( allItems);
            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);
            return root;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            var sortedDraggedIDs = SortItemIDsInRowOrder(args.draggedItemIDs);
            _dragDropHandler.SetupDragAndDrop(sortedDraggedIDs);
        }


        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // First check if the dragged objects are GameObjects
            var draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects.Length == 0)
            {
                return DragAndDropVisualMode.None;
            }


            // Reparent
            if (args.performDrop)
            {
                HandleDropOperation(args);
            }

            return DragAndDropVisualMode.Move;
        }

        private void HandleDropOperation(DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                    _dragDropHandler.HandleDropWithParentItem(args.insertAtIndex, args.parentItem);
                    break;

                case DragAndDropPosition.OutsideItems:
                    _dragDropHandler.HandleDropOutsideRoot(args.insertAtIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Reload();
            // SetSelection (transforms.Select (t => t.gameObject.GetInstanceID ()).ToList (), TreeViewSelectionOptions.RevealAndFrame);
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var expandData = BookMarkDataCenter.instance.ExpandDataMgr.GetExpandData(id);
            if (string.IsNullOrEmpty(expandData.AssetPath )==false)
            {
                var obj = AssetDatabase.LoadAssetAtPath<UnityObject>(expandData.AssetPath);
                EditorGUIUtility.PingObject(obj);
            }
            else
            {
                var bookMarkElement = BookMarkDataCenter.instance.bookMarkDataModel.Find(id);
                if (bookMarkElement != null)
                {
                    if (bookMarkElement.type == BookMarkType.AssetFolder ||
                        bookMarkElement.type == BookMarkType.SingleAsset)
                    {
                        var assetPath =AssetDatabase.GUIDToAssetPath(bookMarkElement.AssetGuild);
                        var obj = AssetDatabase.LoadAssetAtPath<UnityObject>(assetPath);
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
        }
    }
}