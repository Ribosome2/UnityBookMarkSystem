using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LinBookMark
{
    
        public class BookMarkDragDropHandler:IDragDropHandler
        {
            public void SetupDragAndDrop(IList<int> sortedDraggedIDs)
            {
                
                DragAndDrop.PrepareStartDrag ();

                var objList = GetUnityObjectList(sortedDraggedIDs);
                DragAndDrop.objectReferences = objList.ToArray ();
                if (objList.Count > 0)
                {
                    string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
                    DragAndDrop.StartDrag (title);
                }
                else
                {
                    DragAndDrop.StartDrag ("No Unity GameObject");
                }
            }
    
            
            private static List<UnityObject> GetUnityObjectList(IList<int> sortedDraggedIDs)
            {
                List<UnityObject> objList = new List<UnityObject>(sortedDraggedIDs.Count);
                foreach (var id in sortedDraggedIDs)
                {
                    UnityObject obj = EditorUtility.InstanceIDToObject(id);
                    if (obj != null)
                        objList.Add(obj);
                }

                return objList;
            }
            
            private void AddObjectToParent(int insertIndex , UnityObject obj, LinBookMarkElement parentElement,TreeModel<LinBookMarkElement> bookMarkDataModel)
            {
                var addElement = new LinBookMarkElement() {name = obj.name, depth = parentElement.depth + 1, id = TreeItemIdGenerator.NextId};
                Debug.Log("try add to " + parentElement.name);
                if (insertIndex < 0)
                {
                    if (parentElement.hasChildren)
                    {
                        insertIndex = parentElement.children.Count;
                    }
                    else
                    {
                        insertIndex = 0;
                    }
                }

                bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
            }
            
            public void HandleDropWithParentItem(int insertIndex,TreeViewItem parentItem,TreeModel<LinBookMarkElement> bookMarkDataModel)
            {
                var parentElement = bookMarkDataModel.Find(parentItem.id);
                if (parentElement != null)
                {
                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        var obj = DragAndDrop.objectReferences[i];
                        AddObjectToParent(insertIndex, obj, parentElement,bookMarkDataModel);
                    }

                }
            }
            
            public void HandleDropOutsideRoot(int insertIndex,TreeModel<LinBookMarkElement> bookMarkDataModel )
            {
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    var obj = DragAndDrop.objectReferences[i];
                    AddObjectToParent(insertIndex, obj, bookMarkDataModel.root,bookMarkDataModel);
                }
                Debug.Log("drag outside  ");
            }
        }
        

   
}