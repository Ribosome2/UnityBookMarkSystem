using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class DefaultAssetDrawer:IAssetDrawer
    {
        public bool IsValid(string assetPath)
        {
            return true;
        }

        public void DrawAsset(Rect drawRect, string assetPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            var assetIcon = AssetPreview.GetAssetPreview(obj);
            if (assetIcon)
            {
                BookMarkGuiUtil.DrawTexture(drawRect,assetIcon);
            }
            else
            {
                var cachedIcon = AssetDatabase.GetCachedIcon(assetPath);
                if (cachedIcon)
                {
                    assetIcon =(Texture2D) cachedIcon;
                    BookMarkGuiUtil.DrawTexture(drawRect,assetIcon);
                }
            }
        }
    }
}