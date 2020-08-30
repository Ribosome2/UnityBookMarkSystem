using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LinBookMark
{
    public class AssetListGridView
    {
        private int cellSize = 100;
        private int AssetLabelHeight = 22;
        private int heightGap = 125;
        private Vector2 scroll_pos = Vector2.zero;
        private string _curSelectAssetName;
        public string mouseDownAsset;
        public Action<string> ItemClickDelegate;
        public IList<string> showPaths;

        public void OnGUI(Rect drawRect,IList<string> paths,int gridSize)
        {
            cellSize = gridSize;
            showPaths = paths;
            DrawAssets((int)drawRect.width);
        }

        public void DrawAssets(int scrollWidth )
        {
            GUILayout.BeginVertical();
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
                    DrawSingleAsset(row_index, colIndex, showPaths[gridIndex]);
                    ++gridIndex;
                }
                GUILayout.EndHorizontal();
                row_index++;
            }

            GUILayout.Space(row_index * (cellSize + AssetLabelHeight + 2));

            GUILayout.EndScrollView();

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        void DrawSingleAsset(int rowIndex, int colIndex, string assetPath)
        {
            GUILayout.BeginVertical();
            Rect drawRect = new Rect(colIndex * (cellSize + 10),rowIndex * (cellSize + AssetLabelHeight + 2), cellSize, cellSize);
            if (IsVisible(drawRect))
            {
                
                HandleDrawRectInput(drawRect, assetPath);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                var assetIcon = AssetPreview.GetAssetPreview(obj);
                if (assetIcon)
                {
                    BookMarkGuiUtil.DrawTexture(drawRect,assetIcon);
                }
                else
                {
                    assetIcon =(Texture2D) AssetDatabase.GetCachedIcon(assetPath);
                    BookMarkGuiUtil.DrawTexture(drawRect,assetIcon);
                }

                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                var nameRect = new Rect(drawRect.x,drawRect.yMax, cellSize,AssetLabelHeight);
                EditorGUI.TextField(nameRect, "", assetName);
            }
            else
            {
                GUI.Box(drawRect, "Empty");
            }


            GUILayout.EndVertical();

        }

        private void HandleDrawRectInput(Rect drawRect, string assetPath)
        {

            if (Event.current.isMouse && drawRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    mouseDownAsset = assetPath;
                    SetupDrag(assetPath);
                }
                else if (Event.current.type == EventType.MouseUp  && assetPath == mouseDownAsset)
                {
                    if (ItemClickDelegate != null)
                    {
                        ItemClickDelegate.Invoke(assetPath);
                    }
                }
            }
        }

        void SetupDrag(string assetPath)
        {
            DragAndDrop.PrepareStartDrag();
            List<string> paths = new List<string>();
            List<Object> objects = new List<Object>();
            var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (obj)
            {
                paths.Add(AssetDatabase.GetAssetPath(obj));
                objects.Add(obj);
            }
         
            if (objects.Count > 0)
            {
                string title = objects.Count > 1 ? "<Multiple>" : objects.GetType().Name;
                DragAndDrop.StartDrag(title);
            }
            else
            {
                DragAndDrop.StartDrag("nothing to drag");
            }
            DragAndDrop.paths = paths.ToArray();
            DragAndDrop.objectReferences = objects.ToArray();
        }

        private void DrawSprite( Sprite sprite, Rect drawRect)
        {
            Texture2D handleTexture = sprite.texture;
            Rect uv = new Rect(sprite.rect.x / handleTexture.width, sprite.rect.y / handleTexture.height,
                sprite.rect.width / handleTexture.width, sprite.rect.height / handleTexture.height);
            GUI.backgroundColor = new Color(.6f, 1.0f, 1.0f, 0.5f);

            GUI.backgroundColor = Color.white;
            var nameRect = new Rect(drawRect.x,drawRect.yMax, cellSize,AssetLabelHeight);
            EditorGUI.TextField(nameRect, "", sprite.name, "ProgressBarBack");
            if (_curSelectAssetName == sprite.name)
            {
                BookMarkGuiUtil.DrawRectOutline(drawRect, Color.green);
            }

            float spriteWidth = sprite.rect.width;
            float spriteHeight = sprite.rect.height;
            float scale = spriteWidth / spriteHeight;
            drawRect = BookMarkGuiUtil.CalculateDrawRect(spriteWidth, spriteHeight, drawRect);
            GUI.DrawTextureWithTexCoords(drawRect, handleTexture, uv);
           
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

         
    }
}