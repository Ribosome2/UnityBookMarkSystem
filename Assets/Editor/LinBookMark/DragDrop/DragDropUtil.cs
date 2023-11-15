using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
           return path.StartsWith("Assets")==false;
//           return string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path));
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
                        AskRenameAssetOrJustReplace(path,assetPath);
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

        public static bool TryImportDragAssets(string folderPath)
        {
            var needRefresh = false;
            if (string.IsNullOrEmpty(folderPath) == false && Directory.Exists(folderPath))
            {
                foreach (var path in DragAndDrop.paths)
                {
                    if (TryImportOneAsset(folderPath, path))
                    {
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

        private static bool TryImportOneAsset(string folderPath, string path)
        {
            if (File.Exists(path) == false)
            {
                return false;
            }

            var newFileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(newFileName) == false)
            {
                var targetPath = Path.Combine(folderPath, newFileName);
                Debug.Log("target"+targetPath);
                if (File.Exists(targetPath))
                {
                    //drop on asset node ,try replace 
                    if (EditorUtility.DisplayDialog("Add Asset  ",
                        string.Format("Do you want to replace file content of {0} with  file {1}", folderPath, path), "Yes"))
                    {
                        AskRenameAssetOrJustReplace( path, targetPath);
                        return true;
                    }
                }
                else
                {
                    ReplaceFileContent(path, targetPath);
                    return true;
                }
            }

            return false;
        }

        private static void AskRenameAssetOrJustReplace( string path, string targetPath)
        {
            string changeAssetName = null;
            if (Path.GetFileName(path) != Path.GetFileName(targetPath))
            {
                if (EditorUtility.DisplayDialog("Change Name ?",
                    string.Format("Do you want to change AssetName from {0} to  {1}",
                        Path.GetFileName(targetPath), Path.GetFileName(path)), "Use New Name","Keep the  Old Name"))
                {
                    changeAssetName = Path.GetFileName(path);
                }
            }

            ReplaceFileContent(path, targetPath, changeAssetName);
        }

        private static void ReplaceFileContent(string path, string targetPath,string fileNewName = null)
        {
            var newBytes = File.ReadAllBytes(path);
            File.WriteAllBytes(targetPath, newBytes);
            if (string.IsNullOrEmpty(fileNewName) == false)
            {
                AssetDatabase.RenameAsset(targetPath, fileNewName);
            }

            CheckImportAsSprite(targetPath);
        }

        private static void CheckImportAsSprite( string targetPath)
        {
            var slashIndex = targetPath.LastIndexOf('\\');
            var folderPath = targetPath.Substring(0, slashIndex);
            if (AssetMarkDataMgr.AssetsMarkDict.TryGetValue(folderPath, out var assetMark))
            {
                if (assetMark == "Sprite Icon")
                {
                    var extension = Path.GetExtension(targetPath);
                    if (extension == ".png" || extension == ".jpg")
                    {
                        CopyFileAsSprite(targetPath, targetPath);
                    }
                }
            }
        }

        public static void CopyFileAsSprite(string targetPath, string resPath)
        {
            File.WriteAllBytes(targetPath, File.ReadAllBytes(resPath));
            AssetDatabase.ImportAsset(targetPath);
            var importer = TextureImporter.GetAtPath(targetPath) as TextureImporter;
            AssetDatabase.Refresh();
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }


        public static void SetupDragAsset(string assetPath)
        {
            DragAndDrop.PrepareStartDrag();
            List<string> paths = new List<string>();
            List<Object> objects = new List<Object>();
            var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (obj)
            {
                paths.Add(AssetDatabase.GetAssetPath(obj));
                objects.Add(obj);
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
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }

        public static void MoveDraggingAssetToFolder(string parentPath)
        {
            if (Directory.Exists(parentPath) == false)
            {
                Debug.LogWarning(string.Format("Directory: {0} not exist ",parentPath));
                return;
            }

            foreach (var path in DragAndDrop.paths)
            {
                if (string.Equals(parentPath, Path.GetDirectoryName(path),StringComparison.Ordinal) == false)
                {
                    MoveAssetsToFolder(parentPath,path);
                }
            }
        }
        private static void MoveAssetsToFolder(string parentPath , string path)
        {
            var assetObj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (assetObj && string.IsNullOrEmpty(path)==false)
            {
                var newPath = Path.Combine(parentPath, Path.GetFileName(path));
                if (File.Exists(newPath))
                {
                    if (EditorUtility.DisplayDialog("Replace ?", string.Format("Do you want to replace file content of {0} ", newPath), "Yes")==false)
                    {
                        return;
                    }
                }
                AssetDatabase.MoveAsset(path, newPath);
            }
        }
    }
}
