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


        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // First check if the dragged objects are GameObjects
            var draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects.Length == 0 && DragAndDrop.paths.Length==0 )
            {
                return DragAndDropVisualMode.None;
            }


            // Reparent
            if (args.performDrop)
            {
                
            }

            return DragAndDropVisualMode.Move;
        }
    }
}