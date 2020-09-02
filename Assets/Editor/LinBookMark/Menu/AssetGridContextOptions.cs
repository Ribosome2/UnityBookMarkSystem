using System.IO;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class AssetGridContextOptions
    {
         static Object copyObject;
        [MenuItem("KyleKit/Assets/AssetContext/Copy ")]
        public static void CopyAsset(MenuCommand menuCommand)
        {
            copyObject = null;
            if (menuCommand != null)
            {
                copyObject = menuCommand.context;
            }
        }
        
        [MenuItem("KyleKit/Assets/AssetContext/Paste ")]
        public static void PasteAsset(MenuCommand menuCommand)
        {
            if (menuCommand != null && menuCommand.context!=null)
            {
                var oldPath = AssetDatabase.GetAssetPath(copyObject);
                var assetPath = AssetDatabase.GetAssetPath(menuCommand.context);
                var folder = Path.GetDirectoryName(assetPath);
                Debug.Log("ddd"+ oldPath+ " new  "+ folder);
                if (string.IsNullOrEmpty(oldPath) == false && string.IsNullOrEmpty(folder) == false)
                {
                    var newPath = Path.Combine(folder, Path.GetFileName(oldPath));
                    AssetDatabase.CopyAsset(oldPath, newPath);
                }
            }
        }
    }
}