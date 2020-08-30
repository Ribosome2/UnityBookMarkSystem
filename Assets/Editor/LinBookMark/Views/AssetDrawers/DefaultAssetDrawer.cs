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
            if (CheckDrawSprite(drawRect, assetPath)) return;
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

        private static bool CheckDrawSprite(Rect drawRect, string assetPath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite)
            {
                BookMarkGuiUtil.DrawSprite(drawRect, sprite);
                //todo:  find a way to get preview texture type without loading the assets at path 
                var thumbNailIcon = AssetPreview.GetMiniTypeThumbnail(sprite.GetType());
                if (thumbNailIcon)
                {
                    BookMarkGuiUtil.DrawTexture(new Rect(drawRect.x, drawRect.y + drawRect.height * 0.5f, 10, 10),
                        thumbNailIcon);
                }

                return true;
            }

            return false;
        }
    }
}