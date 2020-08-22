using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    static class  DragDropUtil
    {
        public static void FixDragDropFromOutSideProject()
        {
            if (DragAndDrop.paths.Length > 0)
            {
                foreach (var path in DragAndDrop.paths)
                {
                    if (IsNonProjectPath(path))
                    {
                        ClearDragRefObjects();
                    }
                }
            }
        }

        public static void ClearDragRefObjects()
        {
            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
            {
                DragAndDrop.objectReferences= new UnityEngine.Object[0];
            }
        }

        public static bool IsNonProjectPath(string path)
        {
           return string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path));
        }

        public static bool IsDraggingNonProjectPath()
        {
            return DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0;
        }

        public static bool IsDraggingProjectAsset()
        {
            return DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == DragAndDrop.paths.Length;
        }

        public static bool IsDraggingHierarchyObject()
        {
            return DragAndDrop.paths.Length == 0 && DragAndDrop.objectReferences.Length > 0 ;
        }

        public static bool TryReplaceAsset(string assetPath)
        {
            var needRefresh = false;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                foreach (var path in DragAndDrop.paths)
                {
                    //drop on asset node ,try replace 
                    if (EditorUtility.DisplayDialog("replace ",
                        string.Format("Do you want to replace file content of {0} with  file {1}", assetPath, path), "Yes"))
                    {
                        var newBytes = File.ReadAllBytes(path);
                        File.WriteAllBytes(assetPath, newBytes);
                        needRefresh = true;
                    }
                }
                if (needRefresh)
                {
                    AssetDatabase.Refresh();
                }
            }
            return needRefresh;
        }
    }
}
