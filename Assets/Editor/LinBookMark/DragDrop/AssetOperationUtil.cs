using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public static class AssetOperationUtil
    {
        
        public static bool DeleteAssets(IList<int> treeItemIds, bool askIfSure)
        {
            bool flag = DeleteProjectAsset(treeItemIds, askIfSure);
            if (DeleteCustomAddNodes(treeItemIds, askIfSure))
            {
                flag = true;
            }
            return flag;
        }

        static bool DeleteCustomAddNodes(IList<int> treeItemIds, bool askIfSure)
        {
            var list = BookMarkDataCenter.instance.GetNoProjectProjectNodes(treeItemIds);
            if (list.Count == 0)
                return false;
            if (askIfSure)
            {
                string str1 = "Delete selected node";
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
            
            BookMarkDataCenter.instance.bookMarkDataModel.RemoveElements(list);
            BookMarkDataCenter.instance.SaveCurrentTreeModel();
            return true;
        }
        

        private static bool DeleteProjectAsset(IList<int> treeItemIds, bool askIfSure)
        {
            List<string> list = BookMarkDataCenter.instance.GetMainPathsOfAssets(treeItemIds);
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
                Debug.Log("delete asset "+path);
                if (!AssetDatabase.MoveAssetToTrash(path))
                {
                    flag = false;
                }
            }

            AssetDatabase.StopAssetEditing();
            return flag;
        }
    }
}