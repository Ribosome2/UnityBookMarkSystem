using UnityEngine;

namespace LinBookMark
{
    public interface IAssetDrawer
    {
        bool IsValid(string assetPath);
        void DrawAsset(Rect drawRect, string assetPath);
    }
}