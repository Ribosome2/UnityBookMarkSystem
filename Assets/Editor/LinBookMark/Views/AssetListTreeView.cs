using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LinBookMark
{
    public class AssetListTreeView:TreeView
    {
        public IList<string> pathList;
        private EditorWindow parentWindow;
        IAssetContextHandler _assetContextHandler = new AssetContextHandler();
        public AssetListTreeView(TreeViewState state) : base(state)
        {
        }

        protected override void ContextClicked()
        {
            
            base.ContextClicked();
            var parentFolderPath = FileUtil.GetSharedParentFolderPath(pathList);
            if (string.IsNullOrEmpty(parentFolderPath)==false)
            {
                _assetContextHandler.HandlerAssetContextClick(parentFolderPath);
            }
        }

        protected override void ContextClickedItem(int id)
        {
            base.ContextClickedItem(id);
            
            _assetContextHandler.HandlerAssetContextClick(AssetDatabase.GetAssetPath(id));
            Event.current.Use();
        }

        public AssetListTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
        }

        public void SetAssetPathList(IList<string> assetPaths)
        {
            pathList = assetPaths;
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

        protected override void RowGUI(RowGUIArgs args)
        {
            CheckRefreshAssetIcon(args);
            
            CheckDrawPrefabCreate(args);
            base.RowGUI(args);
        }

        private void CheckDrawPrefabCreate(RowGUIArgs args)
        {
            var obj = EditorUtility.InstanceIDToObject(args.item.id);
            if (obj)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                BookMarkGuiUtil.CheckDrawInstantiatePrefabButton(ref assetPath, args.rowRect);
            }
        }

        private static void CheckRefreshAssetIcon(RowGUIArgs args)
        {
            if (args.item.icon == null)
            {
                var obj = EditorUtility.InstanceIDToObject(args.item.id);
                if (obj)
                {
                    args.item.icon = (Texture2D) AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(obj));
                }
            }
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
                result.Add(item);
            }
        }


        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            DragAndDrop.PrepareStartDrag();
            List<string> paths = new List<string>();
            List<Object> objects = new List<Object>();
            foreach (var draggedItemID in args.draggedItemIDs)
            {
                var obj = EditorUtility.InstanceIDToObject(draggedItemID);
                if (obj)
                {
                    paths.Add(AssetDatabase.GetAssetPath(obj));
                    objects.Add(obj);
                }
            }
            
            if (objects.Count > 0)
            {
                string title = objects.Count > 1 ? "<Multiple>" : objects.GetType().Name;
                DragAndDrop.StartDrag(title);
            }
            else
            {
                DragAndDrop.StartDrag("nothing to drag");
            }

            DragAndDrop.paths = paths.ToArray();
            DragAndDrop.objectReferences = objects.ToArray();

        }

        string GetTargetAssetPath(DragAndDropArgs args)
        {
            string result = string.Empty;
            if (args.parentItem != null)
            {
                var obj = EditorUtility.InstanceIDToObject(args.parentItem.id);
                if (obj)
                {
                    result = AssetDatabase.GetAssetPath(obj);
                }
            }
            return result;
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects.Length == 0 && DragAndDrop.paths.Length==0 )
            {
                return DragAndDropVisualMode.None;
            }

            if (args.performDrop)
            {
                DragDropUtil.FixDragDropFromOutSideProject();
                if (DragDropUtil.IsDraggingNonProjectPath())
                {
                    CheckImportAssetByPath(args);
                }else  if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
                {
                    AssetOperationUtil.CreatePrefab(FileUtil.GetSharedParentFolderPath(pathList));
                }
                else
                {
                    HandleOperationInSideProject(args);
                }
            }

            return DragAndDropVisualMode.Move;
        }

        private void HandleOperationInSideProject(DragAndDropArgs args)
        {
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                {
                    DragDropUtil.TryReplaceAsset(AssetDatabase.GetAssetPath(args.parentItem.id));
                    break;
                }
                case DragAndDropPosition.BetweenItems:
                case DragAndDropPosition.OutsideItems:
                    var parentFolderPath = FileUtil.GetSharedParentFolderPath(pathList);
                    if (string.IsNullOrEmpty(parentFolderPath))
                    {
                        EditorUtility.DisplayDialog("add item ?", "Multiple   folders or none  selected ,I don't know where to drop ",
                            "Got it");
                    }
                    else
                    {
                        DragDropUtil.MoveDraggingAssetToFolder(parentFolderPath);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CheckImportAssetByPath(DragAndDropArgs args)
        {
            var assetPath = GetTargetAssetPath(args);
            if (args.dragAndDropPosition == DragAndDropPosition.UponItem)
            {
                DragDropUtil.TryReplaceAsset(assetPath);
            }
            else
            {
                var parentFolderPath = FileUtil.GetSharedParentFolderPath(pathList);
                if (string.IsNullOrEmpty(parentFolderPath))
                {
                    EditorUtility.DisplayDialog("add item ?", "Multiple   folders or none  selected ,I don't know where to drop ",
                        "Got it");
                }
                else
                {
                    DragDropUtil.TryImportDragAssets(parentFolderPath);
                }
            }
        }



        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var obj = EditorUtility.InstanceIDToObject(id);
            EditorGUIUtility.PingObject(obj);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            foreach (var selectedId in selectedIds)
            {
                var obj = EditorUtility.InstanceIDToObject(selectedId);
                if (obj)
                {
                    Selection.SetActiveObjectWithContext(obj,null);
                }
            }
        }




        public string GetParentFolderDesc()
        {
            if (pathList.Count == 1)
            {
                return pathList[0];
            }else if (pathList.Count == 0)
            {
                return "No root selected";
            }
            else
            {
                var shareFolder = FileUtil.GetSharedParentFolderPath(pathList);
                if (string.IsNullOrEmpty(shareFolder))
                {
                    return "Showing MultipleFolders ...";
                }
                else
                {
                    return shareFolder;
                }
            }
        }
    }
}