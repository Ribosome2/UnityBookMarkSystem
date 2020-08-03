using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkDragDropHandler : IDragDropHandler
    {

        public BookMarkDragDropHandler()
        {
            
        }
        
        public void SetupDragAndDrop(IList<int> sortedDraggedIDs)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.paths = new string[0];
            var objList =BookMarkDataCenter.instance. GetUnityObjectList(sortedDraggedIDs);
            DragAndDrop.objectReferences = objList.ToArray();
            if (objList.Count > 0)
            {
                string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
                DragAndDrop.StartDrag(title);
            }
            else
            {
                DragAndDrop.StartDrag("No Unity GameObject");
            }
        }




        private void AddObjectToParent(int insertIndex, UnityObject obj, LinBookMarkElement parentElement)
        {
            var addElement = new LinBookMarkElement()
                {name = obj.name, depth = parentElement.depth + 1, id = TreeItemIdGenerator.NextId};
            Debug.Log("try add to " + parentElement.name);
            insertIndex = CovertInsertIndex(insertIndex, parentElement);
            BookMarkDataCenter.instance.bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
        }

        private static int CovertInsertIndex(int insertIndex, LinBookMarkElement parentElement)
        {
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

            return insertIndex;
        }

        public void HandleDropWithParentItem(int insertIndex, TreeViewItem parentItem)
        {
            var bookMarkDataModel = BookMarkDataCenter.instance.bookMarkDataModel;
            if (DragAndDrop.paths.Length > 0)
            {
                var path = DragAndDrop.paths[0];
                Debug.Log("dddddd "+path);
            }
            var parentElement = bookMarkDataModel.Find(parentItem.id);
            if (parentElement != null)
            {
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    var obj = DragAndDrop.objectReferences[i];
                    AddObjectToParent(insertIndex, obj, parentElement);
                }
            }
        }

        public void HandleDropOutsideRoot(int insertIndex)
        {
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                var obj = DragAndDrop.objectReferences[i];
                AddObjectToParent(insertIndex, obj,BookMarkDataCenter.instance.bookMarkDataModel.root);
            }

            Debug.Log("drag outside  ");
        }
    }
}