using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LinBookMark
{
    public class SpriteAssetClickHandler:IAssetClickHandler
    {
        public bool HandleClickAsset(string assetPath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (sprite)
            {
                SetSpriteToSelectedGameObjectInScene(sprite);
                return true;
            }

            return false;
        }

        private static void SetSpriteToSelectedGameObjectInScene(Sprite sprite)
        {
            foreach (var obj in Selection.objects)
            {
                var go = obj as GameObject;
                if (go)
                {
                    var uiImages = go.GetComponents<Image>();
                    foreach (var uiImage in uiImages)
                    {
                        Undo.RecordObject(uiImage, "changeSprite");
                        uiImage.sprite = sprite;
                    }
                }
            }
        }
    }
}