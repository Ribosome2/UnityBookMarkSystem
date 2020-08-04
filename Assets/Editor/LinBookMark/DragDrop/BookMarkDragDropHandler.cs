﻿using System.Collections.Generic;
using System.IO;
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



        BookMarkType GetBookMarkTypeForAsset(string path)
        {

            if (AssetDatabase.IsValidFolder(path))
            {
                return BookMarkType.AssetFolder;
            }
            else
            {
                return  BookMarkType.SingleAsset;
            }
            return BookMarkType.None;
        }

        LinBookMarkElement ProjectPathToBookMarkElement(string path)
        {
            BookMarkType bookMarkType = GetBookMarkTypeForAsset(path);
            var addElement = new LinBookMarkElement(){type = bookMarkType};
            var assetObject = AssetDatabase.LoadMainAssetAtPath(path);
            if (assetObject)
            {
                Debug.Log("drop path "+ path+" asset type "+bookMarkType +" asset "+ assetObject.GetType());
                addElement.name = assetObject.name;
            }
            addElement.AssetGuild = AssetDatabase.AssetPathToGUID(path);
            return addElement;
        }
        

        private void AddObjectToParent(int insertIndex, LinBookMarkElement parentElement)
        {
            
            if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length== DragAndDrop.paths.Length)
            {
                //drag from project assets 
                var path = DragAndDrop.paths[0];
                BookMarkType bookMarkType = GetBookMarkTypeForAsset(path);
                Debug.Log("drop path "+ path+" asset type "+bookMarkType );
                var addElement = ProjectPathToBookMarkElement(path);
                addElement.depth = parentElement.depth + 1;
                addElement.id = TreeItemIdGenerator.NextId;
                insertIndex = CovertInsertIndex(insertIndex, parentElement);
                Debug.Log("try add to " + parentElement.name);
                BookMarkDataCenter.instance.bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
                
            }
            else
            {
                //not project assets 
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    var obj = DragAndDrop.objectReferences[i];
                    var addElement = new LinBookMarkElement()
                        {name = obj.name, depth = parentElement.depth + 1, id = TreeItemIdGenerator.NextId};
                    Debug.Log("try add to " + parentElement.name);
                    insertIndex = CovertInsertIndex(insertIndex, parentElement);
                    BookMarkDataCenter.instance.bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
                }
            }
            
          
            
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

            var parentElement = bookMarkDataModel.Find(parentItem.id);
            if (parentElement != null)
            {
                AddObjectToParent(insertIndex, parentElement);
            }
        }

        public void HandleDropOutsideRoot(int insertIndex)
        {
         
            AddObjectToParent(insertIndex,BookMarkDataCenter.instance.bookMarkDataModel.root);
            Debug.Log("drag outside  ");
        }
    }
}