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
        private TreeModel<LinBookMarkElement> bookMarkDataModel;
        BookMarkTreeBuilder _treeBuilder = new BookMarkTreeBuilder();
        BookMarkDragDropHandler _dragDropHandler = new BookMarkDragDropHandler();
        List<LinBookMarkElement> bookMarks = new List<LinBookMarkElement>();


        private void InitBookMarkList()
        {
            var bookMark = new LinBookMarkElement() {name = "Root", depth = -1, id = TreeItemIdGenerator.NextId};
            bookMarks.Add(bookMark);
            var bookM2 = new LinBookMarkElement() {name = "customChild1", depth = 0, id = TreeItemIdGenerator.NextId};
            bookMarks.Add(bookM2);
            bookMarks.Add(new LinBookMarkElement() {name = "Kyle ", depth = 1, id = TreeItemIdGenerator.NextId});
        }

        public LinBookMarkTreeView(TreeViewState treeViewState)
            : base(treeViewState)
        {
            InitBookMarkList();
            bookMarkDataModel = new TreeModel<LinBookMarkElement>(bookMarks);
            Reload();
        }


        protected override TreeViewItem BuildRoot()
        {
            // BuildRoot is called every time Reload is called to ensure
            List<TreeViewItem> allItems = new List<TreeViewItem>();
            var root = _treeBuilder.BuildRoot(bookMarkDataModel, allItems);
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
                    _dragDropHandler.HandleDropWithParentItem(args.insertAtIndex, args.parentItem, bookMarkDataModel);
                    break;

                case DragAndDropPosition.OutsideItems:
                    _dragDropHandler.HandleDropOutsideRoot(args.insertAtIndex, bookMarkDataModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Reload();
            // SetSelection (transforms.Select (t => t.gameObject.GetInstanceID ()).ToList (), TreeViewSelectionOptions.RevealAndFrame);
        }
    }
}