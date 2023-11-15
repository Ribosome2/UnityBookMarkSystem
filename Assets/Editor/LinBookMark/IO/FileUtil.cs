using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public static  class FileUtil
    {
        public static void CheckPasteFiles(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
            {
                Event.current.Use();
                var paths = GUIUtility.systemCopyBuffer.Split(',');
                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        var newPath = Path.Combine(folderPath, Path.GetFileName(path));
                        AssetDatabase.CopyAsset(path, newPath);
                    }
                }
            }
        }

        public static IList<string> GetAssetFileList(IList<string> pathList)
        {
            List<string> result = new List<string>();
            foreach (var path in pathList)
            {
                if (Directory.Exists(path))
                {
                    var subFiles = Directory.GetFiles(path).Where(x => x.EndsWith(".meta")==false);
                    foreach (var subFile in subFiles)
                    {
                        result.Add(subFile);
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        result.Add(path);
                    }
                }
            }

            return result;
        }

        public static string GetSharedParentFolderPath(IList<string> pathList)
        {
            var result = string.Empty;
            foreach (var path in pathList)
            {
                var parentPath = GetFolderPathFromPathString(path);
                if (string.IsNullOrEmpty(parentPath)==false)
                {
                    
                    if (string.IsNullOrEmpty(result))
                    {
                        result = parentPath;
                    }
                    else
                    {
                        if (parentPath != result)
                        {
                            // only return the parent path when all asset share same one
                            return string.Empty;
                        }
                    }
                }
            }
            return result;
        }
        
        private static string GetFolderPathFromPathString(string path)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
            else
            {
                if (File.Exists(path))
                {
                    return Path.GetDirectoryName(path);
                }
            }
            return string.Empty;
        }

    }
}