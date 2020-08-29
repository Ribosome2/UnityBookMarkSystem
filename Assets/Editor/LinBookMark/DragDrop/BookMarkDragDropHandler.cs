using System.Collections.Generic;
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

        public void SetupDragAndDrop(IList<int> sortedDraggedIDs)
        {
            
            DragAndDrop.PrepareStartDrag();

            List<Object> objList;
            var pathList = ExtractObjectListAndPathList(sortedDraggedIDs, out objList);

            DragAndDrop.paths = pathList.ToArray();
            DragAndDrop.objectReferences = objList.ToArray();
            if (objList.Count > 0)
            {
                string title = objList.Count > 1 ? "<Multiple>" : objList.GetType().Name;
                DragAndDrop.StartDrag(title);
            }
            else
            {
                DragAndDrop.StartDrag("No Unity GameObject");
            }
        }

        private static List<string> ExtractObjectListAndPathList(IList<int> sortedDraggedIDs, out List<Object> objList)
        {
            List<string> pathList = new List<string>();
            objList = new List<Object>();
            foreach (int draggedId in sortedDraggedIDs)
            {
                var expandData = BookMarkDataCenter.instance.ExpandDataMgr.GetExpandData(draggedId);
                if (string.IsNullOrEmpty(expandData.AssetPath) == false)
                {
                    var assetPath = expandData.AssetPath;
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    if (obj)
                    {
                        pathList.Add(assetPath);
                        objList.Add(obj);
                    }
                }
                else
                {
                    var element = BookMarkDataCenter.instance.bookMarkDataModel.Find(draggedId);
                    if (element != null)
                    {
                        DragAndDrop.SetGenericData("BookMarkNodeDragging",draggedId);
                    }
                }
            }

            return pathList;
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
                //drag from project browser
                foreach (var path in DragAndDrop.paths)
                {
                    insertIndex = HandleDropToParentTreeItem(insertIndex, parentElement, path);
                }
            }else if (DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0)
            {
                CheckCreatePrefabFromHierarchy(parentElement);
            }
            else
            {
                insertIndex = CovertInsertIndex(insertIndex, parentElement);
                //not project assets 
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                {
                    var obj = DragAndDrop.objectReferences[i];
                    if (obj == null)
                    {
                        Debug.LogError("null obj, WTH ?");
                        continue;
                    }
                        
                    var addElement = new LinBookMarkElement()
                        {name = obj.name, depth = parentElement.depth + 1, id = TreeItemIdGenerator.NextId};
                    BookMarkDataCenter.instance.bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
                }

                CheckDropCustomBookMarkNode(insertIndex, parentElement);
            }
            
          
            
        }

        private static void CheckCreatePrefabFromHierarchy(LinBookMarkElement parentElement)
        {
            var parentProjectPath = parentElement.GetProjectPath();
            AssetOperationUtil.CreatePrefab(parentProjectPath);
        }



        private static void CheckDropCustomBookMarkNode(int insertIndex, LinBookMarkElement parentElement)
        {
            var dragData = DragAndDrop.GetGenericData("BookMarkNodeDragging");
            if (dragData != null)
            {
                int dragStartId = (int) dragData;
                if (dragStartId != 0 && parentElement.type== BookMarkType.CustomRoot)
                {
                    List<TreeElement> elements = new List<TreeElement>();
                    var startItem = BookMarkDataCenter.instance.bookMarkDataModel.Find(dragStartId);
                    if (startItem != null)
                    {
                        elements.Add(startItem);
                        BookMarkDataCenter.instance.bookMarkDataModel.MoveElements(parentElement, insertIndex, elements);
                    }
                }
            }
        }

        private int HandleDropToParentTreeItem(int insertIndex, LinBookMarkElement parentElement, string path)
        {
            if (parentElement.type == BookMarkType.AssetFolder)
            {
                MoveAssets(parentElement, path);
            }
            else if (parentElement.type == BookMarkType.CustomRoot)
            {
                var addElement = ProjectPathToBookMarkElement(path);
                addElement.depth = parentElement.depth + 1;
                addElement.id = TreeItemIdGenerator.NextId;
                insertIndex = CovertInsertIndex(insertIndex, parentElement);
                BookMarkDataCenter.instance.bookMarkDataModel.AddElement(addElement, parentElement, insertIndex);
                BookMarkDataCenter.instance.SaveCurrentTreeModel();
            }
            return insertIndex;
        }

        private static void MoveAssets(LinBookMarkElement parentElement, string path)
        {
            var assetObj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (assetObj)
            {
                var fileInfo = new FileInfo(path);
                var parentPath = AssetDatabase.GUIDToAssetPath(parentElement.AssetGuild);
                var newPath = Path.Combine(parentPath, fileInfo.Name);
                AssetDatabase.MoveAsset(path, newPath);
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
            else
            {
                Debug.LogError("Can't find parent with Id "+parentItem.id);
            }
        }

        public void HandleDropOutsideRoot(int insertIndex)
        {
         
            AddObjectToParent(insertIndex,BookMarkDataCenter.instance.bookMarkDataModel.root);
            Debug.Log("drag outside  ");
        }

        public void CheckDropToReplace(int treeItemId)
        {
            var assetPath = BookMarkDataCenter.instance.GetAssetPath(treeItemId);
            DragDropUtil.TryReplaceAsset(assetPath);
        }
    }
}