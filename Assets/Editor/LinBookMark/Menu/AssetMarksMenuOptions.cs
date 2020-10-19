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
        
        [MenuItem("KyleKit/Assets/AssetMark/MarkAsParticleFolder ")]
        public static void MarkAsParticleFolder(MenuCommand menuCommand)
        {
            var assetIcon = "d_ParticleSystem Icon";
            MarkSelectedAssetPaths(assetIcon);
        }
        
        [MenuItem("KyleKit/Assets/AssetMark/MarkAsTextureFolder ")]
        public static void MarkAsTextureFolder(MenuCommand menuCommand)
        {
            var assetIcon = "RawImage Icon";
            MarkSelectedAssetPaths(assetIcon);
        }
        
        [MenuItem("KyleKit/Assets/AssetMark/MarkAsTextFolder ")]
        public static void MarkAsTextFolder(MenuCommand menuCommand)
        {
            var assetIcon = "Text Icon";
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