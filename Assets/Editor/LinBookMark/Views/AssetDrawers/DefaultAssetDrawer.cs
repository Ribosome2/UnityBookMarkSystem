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
                // BookMarkGuiUtil.Draw(drawRect,assetIcon);
            }
            else
            {
                    
            }
        }
    }
}