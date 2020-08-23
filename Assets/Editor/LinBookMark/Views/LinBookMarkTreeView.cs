using System;
using System.Collections.Generic;
using System.IO;
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
        public Action<IList<int>> OnSelectionChange;

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
        
        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            
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
            if (draggedObjects.Length == 0 && DragAndDrop.paths.Length==0 && DragAndDrop.GetGenericData("BookMarkNodeDragging")==null)
            {
                return DragAndDropVisualMode.None;
            }


            // Reparent
            if (args.performDrop)
            {
                DragDropUtil.FixDragDropFromOutSideProject();
                HandleDropOperation(args);
            }

            return DragAndDropVisualMode.Move;
        }

        private void CheckImportAssetByPath(DragAndDropArgs args)
        {
            if (args.parentItem != null)
            {
                var assetPath = BookMarkDataCenter.instance.GetAssetPath(args.parentItem.id);
                if (args.dragAndDropPosition == DragAndDropPosition.UponItem && Path.HasExtension(assetPath))
                {
                    _dragDropHandler.CheckDropToReplace(args.parentItem.id);
                }
                else
                {
                    DragDropUtil.TryImportDragAssets(assetPath);
                }
            }
           
        }

        private void HandleDropOperation(DragAndDropArgs args)
        {
            if (DragDropUtil.IsDraggingNonProjectPath())
            {
                CheckImportAssetByPath(args);
            }
            else
            {
                HandleOperationInSideProject(args);
            }

            Reload();
            // SetSelection (transforms.Select (t => t.gameObject.GetInstanceID ()).ToList (), TreeViewSelectionOptions.RevealAndFrame);
        }

        private void HandleOperationInSideProject(DragAndDropArgs args)
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
            BookMarkDataCenter.instance.SaveCurrentTreeModel();
        }

        protected override bool CanRename(TreeViewItem item)
        {
            var element = BookMarkDataCenter.instance.bookMarkDataModel.Find(item.id);
            if (element != null && element.type == BookMarkType.CustomRoot)
            {
                return true;
            }
            
            return base.CanRename(item);
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);
            var element = BookMarkDataCenter.instance.bookMarkDataModel.Find(args.itemID);
            if (element != null && element.type == BookMarkType.CustomRoot)
            {
                element.name = args.newName;
                Reload();
                BookMarkDataCenter.instance.SaveCurrentTreeModel();
            }
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

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (OnSelectionChange != null)
            {
                OnSelectionChange(selectedIds);
            }
            
        }
    }
}