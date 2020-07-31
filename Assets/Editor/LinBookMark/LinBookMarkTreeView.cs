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
        List<LinBookMarkElement> bookMarks = new List<LinBookMarkElement>();
        private int autoId = 1;

        private void InitBookMarkList()
        {
            var bookMark = new LinBookMarkElement(){name = "Root",depth = -1};
            bookMarks.Add(bookMark);
            var bookM2 = new LinBookMarkElement(){name = "Rooeeet",depth = 0};
            bookMarks.Add(bookM2);
        }
        
            public LinBookMarkTreeView(TreeViewState treeViewState)
                : base(treeViewState)
            {
                InitBookMarkList();
                bookMarkDataModel =new TreeModel<LinBookMarkElement>(bookMarks);
                Reload();
            }
		
            
            static TreeViewItem CreateTreeViewItemForBookMarkElement (LinBookMarkElement element)
            {
                // We can use the GameObject instanceID for TreeViewItem id, as it ensured to be unique among other items in the tree.
                // To optimize reload time we could delay fetching the transform.name until it used for rendering (prevents allocating strings 
                // for items not rendered in large trees)
                // We just set depth to -1 here and then call SetupDepthsFromParentsAndChildren at the end of BuildRootAndRows to set the depths.
                return new TreeViewItem(element.id, element.depth, element.name);
            }
            
            protected override TreeViewItem BuildRoot ()
            {
                // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
                // are created from data. Here we just create a fixed set of items, in a real world example
                // a data model should be passed into the TreeView and the items created from the model.

                // This section illustrates that IDs should be unique and that the root item is required to 
                // have a depth of -1 and the rest of the items increment from that.
                var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
                var allItems = new List<TreeViewItem>();
                int Id = 1;
                for (int i = 0; i < bookMarks.Count; i++)
                {
                    var element = bookMarks[i];
                    var treeItem = CreateTreeViewItemForBookMarkElement(element);
                    Debug.Log("re add "+element.name);
                    allItems.Add(treeItem);
                }
                
			
                // Utility method that initializes the TreeViewItem.children and -parent for all items.
                SetupParentsAndChildrenFromDepths (root, allItems);
			
                // Return root of the tree
                return root;
            }
            
            protected override bool CanStartDrag (CanStartDragArgs args)
            {
                return true;
            }
            
            protected override void SetupDragAndDrop (SetupDragAndDropArgs args)
            {
                DragAndDrop.PrepareStartDrag ();

                var sortedDraggedIDs = SortItemIDsInRowOrder (args.draggedItemIDs);

                List<UnityObject> objList = new List<UnityObject> (sortedDraggedIDs.Count);
                foreach (var id in sortedDraggedIDs)
                {
                    UnityObject obj = EditorUtility.InstanceIDToObject (id);
                    if (obj != null)
                        objList.Add (obj);
                }

                DragAndDrop.objectReferences = objList.ToArray ();

                string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
                DragAndDrop.StartDrag (title);
            }
            
            protected override DragAndDropVisualMode HandleDragAndDrop (DragAndDropArgs args)
            {
                // First check if the dragged objects are GameObjects
                var draggedObjects = DragAndDrop.objectReferences;
                if (draggedObjects.Length == 0)
                {
                    return DragAndDropVisualMode.None;
                }

                // // Filter out any unnecessary transforms before the reparent operation
                // RemoveItemsThatAreDescendantsFromOtherItems (transforms);

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
                        for (int i = 0; i <  DragAndDrop.objectReferences.Length; i++)
                        {
                            var obj = DragAndDrop.objectReferences[i];
                            var addElement  = new LinBookMarkElement(){name = obj.name,depth = args.parentItem.depth+1,id = 44};
                            bookMarks.Add(addElement);
                        }
                        Debug.Log("dragDrop  " + args.parentItem.id);
                        Reload();
                        break;
                        
                    case DragAndDropPosition.OutsideItems:
                        Debug.Log("drag outside  ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Reload();
                // SetSelection (transforms.Select (t => t.gameObject.GetInstanceID ()).ToList (), TreeViewSelectionOptions.RevealAndFrame);
            }
    }
}