using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public static class AssetOperationUtil
    {

        static List<string> GetMainPathsOfAssets(IList<int> treeItemIds)
        {
            List<string> result = new List<string>();
            foreach (int treeItemId in treeItemIds)
            {
                var item = BookMarkDataCenter.instance.ExpandDataMgr.GetExpandData(treeItemId);
                if (!string.IsNullOrEmpty(item.AssetPath) )
                {
                    var path = AssetDatabase.GUIDToAssetPath(item.AssetPath);
                    result.Add(path);
                }
            }
            
            return result;
        }
        
        public static bool DeleteAssets(IList<int> treeItemIds, bool askIfSure)
        {
            Debug.Log("delete ");
            if (treeItemIds.Count == 0)
                return true;

            return DeleteProjectAsset(treeItemIds, askIfSure);
        }

        private static bool DeleteProjectAsset(IList<int> treeItemIds, bool askIfSure)
        {
            List<string> list = GetMainPathsOfAssets(treeItemIds);
            if (list.Count == 0)
                return false;
            if (askIfSure)
            {
                string str1 = "Delete selected asset";
                if (list.Count > 1)
                    str1 += "s";
                string title = str1 + "?";
                int num = 3;
                string str2 = "";
                for (int index = 0; index < list.Count && index < num; ++index)
                    str2 = str2 + "   " + list[index] + "\n";
                if (list.Count > num)
                    str2 += "   ...\n";
                string message = str2 + "\nYou cannot undo this action.";
                if (!EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
                    return false;
            }

            bool flag = true;
            AssetDatabase.StartAssetEditing();
            foreach (string path in list)
            {
                if (!AssetDatabase.MoveAssetToTrash(path))
                    flag = false;
            }

            AssetDatabase.StopAssetEditing();
            return flag;
        }
    }
}