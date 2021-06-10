using LinBookMark;
using UnityEditor;

namespace LinBookMark
{
    public  class AssetMarksMenuOptions
    {
        
        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/ClearMark")]
        public static void ClearAssetMark(MenuCommand menuCommand)
        {
            var assetIcon = string.Empty;
            MarkSelectedAssetPaths(assetIcon);
        }

        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/Sprite")]
        public static void MarkAsSpriteFolder(MenuCommand menuCommand)
        {
            var assetIcon = "Sprite Icon";
            MarkSelectedAssetPaths(assetIcon);
        }

        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/Prefab ")]
        public static void MarkAsPrefabFolder(MenuCommand menuCommand)
        {
            var assetIcon = "PrefabNormal Icon";
            MarkSelectedAssetPaths(assetIcon);
        }
        
        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/Particle ")]
        public static void MarkAsParticleFolder(MenuCommand menuCommand)
        {
            var assetIcon = "d_ParticleSystem Icon";
            MarkSelectedAssetPaths(assetIcon);
        }
        
        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/Texture ")]
        public static void MarkAsTextureFolder(MenuCommand menuCommand)
        {
            var assetIcon = "RawImage Icon";
            MarkSelectedAssetPaths(assetIcon);
        }
        
        [MenuItem("CONTEXT/KyleKit/Assets/AssetMark/Text ")]
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