using LinBookMark;
using UnityEditor;

namespace LinBookMark
{
    public  class AssetMarksMenuOptions
    {
        
        [MenuItem("KyleKit/Assets/AssetMark/ClearMark")]
        public static void ClearAssetMark(MenuCommand menuCommand)
        {
            var assetIcon = string.Empty;
            MarkSelectedAssetPaths(assetIcon);
        }

        [MenuItem("KyleKit/Assets/AssetMark/MarkAsSpriteFolder")]
        public static void MarkAsSpriteFolder(MenuCommand menuCommand)
        {
            var assetIcon = "Sprite Icon";
            MarkSelectedAssetPaths(assetIcon);
        }

        [MenuItem("KyleKit/Assets/AssetMark/MarkAsPrefabFolder ")]
        public static void MarkAsPrefabFolder(MenuCommand menuCommand)
        {
            var assetIcon = "PrefabNormal Icon";
            MarkSelectedAssetPaths(assetIcon);
        }

        private static void MarkSelectedAssetPaths(string assetIcon)
        {
            if (KyleSelections.assetGUIDs != null)
            {
                foreach (var guild in KyleSelections.assetGUIDs)
                {
                    if (string.IsNullOrEmpty(guild) == false)
                    {
                        if (string.IsNullOrEmpty(assetIcon))
                        {
                            AssetMarkDataMgr.AssetsMarkDict.Remove(guild);
                        }
                        else
                        {
                            AssetMarkDataMgr.AssetsMarkDict[guild] = assetIcon;
                        }
                    }
                }
            }
            AssetMarkDataMgr.Save();
        }
    }
}