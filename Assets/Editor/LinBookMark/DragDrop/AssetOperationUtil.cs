using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public static class AssetOperationUtil
    {
        
        public static bool DeleteAssets(IList<int> treeItemIds, bool askIfSure,ITreeViewIdConverter idConverter)
        {
            bool flag = DeleteProjectAsset(treeItemIds, askIfSure);
            if (DeleteCustomAddNodes(treeItemIds, askIfSure,idConverter))
            {
                flag = true;
            }
            return flag;
        }

        static bool DeleteCustomAddNodes(IList<int> treeItemIds, bool askIfSure ,ITreeViewIdConverter idConverter)
        {
            var list = BookMarkDataCenter.instance.GetNoProjectProjectNodes(treeItemIds);
            if (list.Count == 0)
                return false;
            if (askIfSure)
            {
                string titleDesc = "Delete selected node";
                if (list.Count > 1)
                    titleDesc += "s";
                string title = titleDesc + "?";
                int num = 3;
                string message = "delete : \n";
                for (int index = 0; index < list.Count && index < num; ++index)
                {
                    message = message + "   " + idConverter.GetItemName(list[index]) + "\n";
                }

                if (list.Count > num)
                {
                    message += "   ...\n";
                }
                message = message + "\nYou cannot undo this action.";
                if (!EditorUtility.DisplayDialog(title, message, "Delete", "Cancel"))
                    return false;
            }
            
            BookMarkDataCenter.instance.bookMarkDataModel.RemoveElements(list);
            BookMarkDataCenter.instance.SaveCurrentTreeModel();
            return true;
        }

        private static bool DeleteProjectAsset(IList<int> treeItemIds, bool askIfSure)
        {
            List<string> list = BookMarkDataCenter.instance.GetMainPathsOfAssetsFromAutoExpandNodes(treeItemIds);
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
        
        public static void CreatePrefab(string parentProjectPath)
        {
            if (string.IsNullOrEmpty(parentProjectPath) == false)
            {
                foreach (var objectReference in DragAndDrop.objectReferences)
                {
                    var go = objectReference as GameObject;
                    if (go)
                    {
                        var prefabPath = string.Format("{0}/{1}.prefab", parentProjectPath, go.name);
                        var prefab = PrefabUtility.CreatePrefab(prefabPath, go);
                        PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
                    }
                }
            }
        }
    }
}