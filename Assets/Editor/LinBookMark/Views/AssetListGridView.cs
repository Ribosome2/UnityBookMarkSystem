using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LinBookMark
{
    public class AssetListGridView
    {
        private int cellSize = 100;
        private int SPRITE_LABEL_HEIGHT = 22;
        private int heightGap = 125;
        private Vector2 scroll_pos = Vector2.zero;
        private string cur_select_sprite_name;
        public Action<Object> ItemClickDelegate;
        public bool UseMinSize = true;
        public IList<string> showPaths;


        public void OnGUI(Rect drawRect,IList<string> paths,int gridSize)
        {
            cellSize = gridSize;
            showPaths = paths;
            DrawSprites((int)drawRect.width);
        }

        public void DrawSprites(int scrollWidth )
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Space(10);
            scrollWidth = Mathf.Max(100, scrollWidth);
            scroll_pos = GUILayout.BeginScrollView(scroll_pos, GUILayout.Width(scrollWidth));

            int colShowCount = (scrollWidth) / (cellSize + 10);
            colShowCount = Mathf.Max(1, colShowCount); //the following for-loop will run forever if we limit this
            int totalCount = showPaths.Count;
            int row_index = 0;
            for (int gridIndex = 0; gridIndex < totalCount; )
            {
                GUILayout.BeginHorizontal();
                for (int colIndex = 0; colIndex < colShowCount && gridIndex < totalCount; ++colIndex)
                {
                    DrawSprite(row_index, colIndex, showPaths[gridIndex]);
                    ++gridIndex;
                }
                GUILayout.EndHorizontal();
                row_index++;
            }

            GUILayout.Space(row_index * (cellSize + SPRITE_LABEL_HEIGHT + 2));

            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        void DrawSprite(int rowIndex, int colIndex, string assetPath)
        {
            GUILayout.BeginVertical();

            Rect drawRect = new Rect(colIndex * (cellSize + 10),rowIndex * (cellSize + SPRITE_LABEL_HEIGHT + 2), cellSize, cellSize);

            if (IsVisible(drawRect))
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                var assetIcon = AssetPreview.GetAssetPreview(obj);
                if (assetIcon)
                {
                    DrawTexture(drawRect,assetIcon);
                }
                // var sprite = LoadAssetByPath<Sprite>(spritePath);
                // var assetObj = LoadAssetByPath<Object>(spritePath);
                // if (sprite)
                // {
                //     DrawSprite(sprite, drawRect);
                // }
                // else
                // {
                //     DrawTexture(spritePath, drawRect);
                // }

            }
            else
            {
                GUI.Box(drawRect, "Empty");
            }


            GUILayout.EndVertical();

        }

        private void DrawTexture(string spritePath, Rect drawRect)
        {
            var texture = LoadAssetByPath<Texture>(spritePath);
            if (texture)
            {
                DrawTexture(drawRect, texture);
            }
        }

        private void DrawTexture(Rect drawRect, Texture texture)
        {
            DrawRectOutline(drawRect, Color.red);
            float textureWidth = texture.width;
            float textureHeight = texture.height;
            float scale = textureWidth / textureWidth;
            drawRect = CalculateDrawRect(textureWidth, textureHeight, drawRect, scale);
            GUI.DrawTexture(drawRect, texture);
        }

        private void DrawSprite( Sprite sprite, Rect drawRect)
        {
            Texture2D handle_texture = sprite.texture;
            Rect uv = new Rect(sprite.rect.x / handle_texture.width, sprite.rect.y / handle_texture.height,
                sprite.rect.width / handle_texture.width, sprite.rect.height / handle_texture.height);
            GUI.backgroundColor = new Color(.6f, 1.0f, 1.0f, 0.5f);
            if (GUI.Button(drawRect, ""))
            {
                if (ItemClickDelegate != null)
                {
                    ItemClickDelegate.Invoke(sprite);
                }
            }

            GUI.backgroundColor = Color.white;
            var nameRect = new Rect(drawRect.x,drawRect.yMax, cellSize,SPRITE_LABEL_HEIGHT);
            EditorGUI.TextField(nameRect, "", sprite.name, "ProgressBarBack");
            if (cur_select_sprite_name == sprite.name)
            {
                DrawRectOutline(drawRect, Color.green);
            }

            float spriteWidth = sprite.rect.width;
            float spriteHeight = sprite.rect.height;
            float scale = spriteWidth / spriteHeight;
            drawRect = CalculateDrawRect(spriteWidth, spriteHeight, drawRect, scale);

            GUI.DrawTextureWithTexCoords(drawRect, handle_texture, uv);
           
        }

        private Rect CalculateDrawRect(float spriteWidth, float spriteHeight, Rect draw_rect, float scale)
        {
            if (spriteWidth < spriteHeight)
            {
                draw_rect.height = UseMinSize ? Mathf.Min(spriteHeight, cellSize):cellSize;
                draw_rect.width = draw_rect.height * scale;
            }
            else
            {
                draw_rect.width =UseMinSize ? Mathf.Min(spriteWidth,cellSize):cellSize;
                draw_rect.height = draw_rect.width / scale;
            }
            draw_rect.x += (cellSize - draw_rect.width) / 2;
            draw_rect.y += (cellSize - draw_rect.height) / 2;
            return draw_rect;
        }

        bool IsVisible(Rect drawRect)
         {
             // if (drawRect.y + heightGap + cellSize >= scroll_pos.y && drawRect.y < scroll_pos.y + mMaxHeight) 
             {
                 return true;
             }
             return false;
         }
         public  T LoadAssetByPath<T>(string spritePath) where T:Object
         {
             return AssetDatabase.LoadAssetAtPath<T>("Assets" +spritePath);
         }
         public void DrawRectOutline(Rect rect, Color color)
         {
             if (Event.current.type == EventType.Repaint)
             {
                 Texture2D tex = EditorGUIUtility.whiteTexture;
                 GUI.color = color;
                 GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
                 GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
                 GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                 GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
                 GUI.color = Color.white;
             }
         }
         
    }
}