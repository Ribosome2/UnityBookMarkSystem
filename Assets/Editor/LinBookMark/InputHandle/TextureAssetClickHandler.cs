using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LinBookMark
{
    public class TextureAssetClickHandler:IAssetClickHandler
    {
        public bool HandleClickAsset(string assetPath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
            if (sprite)
            {
                SetSpriteToSelectedGameObjectInScene(sprite);
                return true;
            }

            return false;
        }

        private static void SetSpriteToSelectedGameObjectInScene(Texture sprite)
        {
            foreach (var obj in Selection.objects)
            {
                var go = obj as GameObject;
                if (go)
                {
                    var uiImages = go.GetComponents<RawImage>();
                    foreach (var uiImage in uiImages)
                    {
                        Undo.RecordObject(uiImage, "changeRawImageTexture");
                        uiImage.texture = sprite;
                    }
                }
            }
        }
    }
}