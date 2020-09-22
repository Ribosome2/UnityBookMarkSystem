﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            CheckRefreshIcon(args);
            base.RowGUI(args);
            var path = BookMarkDataCenter.instance.GetAssetPath(args.item.id);
            if (string.IsNullOrEmpty(path) == false)
            {
                string assetMark;
                // path = AssetDatabase.AssetPathToGUID(path);
                if (AssetMarkDataMgr.AssetsMarkDict.TryGetValue(path, out assetMark))
                {
                    var icon = (Texture2D) EditorGUIUtility.Load(assetMark);
                    if (icon)
                    {
                        var rowRect = args.rowRect;
                        var iconSize = rowRect.height;
                        var drawRect = new Rect(rowRect.x+rowRect.width-rowRect.height,rowRect.y,iconSize,iconSize);
                        BookMarkGuiUtil.DrawTexture(drawRect, icon);
                    }
                }
            }
        }

        private static void CheckRefreshIcon(RowGUIArgs args)
        {
            if (args.item.icon == null)
            {
                var path = BookMarkDataCenter.instance.GetAssetPath(args.item.id);
                if (string.IsNullOrEmpty(path) == false)
                {
                    args.item.icon =(Texture2D) AssetDatabase.GetCachedIcon(path);
                }
            }
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

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
            var rect = new Rect(Event.current.mousePosition.x,Event.current.mousePosition.y,100,16);
            EditorUtility.DisplayPopupMenu(rect, "KyleKit/Assets", (MenuCommand) null);
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
            var projectPath = BookMarkDataCenter.instance.GetAssetPath(id);
            if (string.IsNullOrEmpty(projectPath )==false)
            {
                if (Path.GetExtension(projectPath) == ".unity")
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(projectPath);
                    }
                }
                else
                {
                    var obj = AssetDatabase.LoadAssetAtPath<UnityObject>(projectPath);
                    EditorGUIUtility.PingObject(obj);
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