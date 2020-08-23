using System.IO;
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
    }
}