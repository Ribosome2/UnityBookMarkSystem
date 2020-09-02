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
        private Rect _drawRect;
        private IAssetDrawer _assetDrawer = new DefaultAssetDrawer();
        private string _selectAssetPath;
        public void OnGUI(Rect drawRect,IList<string> paths,int gridSize)
        {
            _drawRect = drawRect;
            cellSize = gridSize;
            showPaths = paths;
            DrawAssets((int)drawRect.width);

            CheckDropToSelectFolder(drawRect);
        }

        private void CheckDropToSelectFolder(Rect drawRect)
        {
            if (Event.current.type == EventType.DragPerform && drawRect.Contains(Event.current.mousePosition))
            {
                var parentFolder = FileUtil.GetSharedParentFolderPath(showPaths);
                if (string.IsNullOrEmpty(parentFolder) == false)
                {
                    if (DragDropUtil.IsDraggingProjectAsset())
                    {
                        DragDropUtil.MoveDraggingAssetToFolder(parentFolder);
                    }
                    else if(DragDropUtil.IsDraggingNonProjectPath())
                    {
                        DragDropUtil.TryImportDragAssets(parentFolder);
                    }else if (DragDropUtil.IsDraggingHierarchyObject())
                    {
                        AssetOperationUtil.CreatePrefab(parentFolder);
                    }
                    Event.current.Use();
                }
            }
        }

        private void DrawAssets(int scrollWidth )
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
                DrawVisibleAsset(assetPath, drawRect);
            }
            else
            {
                GUI.Box(drawRect, "Empty");
            }
            GUILayout.EndVertical();
        }

        private void DrawVisibleAsset(string assetPath, Rect drawRect)
        {
            HandleDrawRectInput(drawRect, assetPath);
            _assetDrawer.DrawAsset(drawRect, assetPath);
            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            var nameRect = new Rect(drawRect.x, drawRect.yMax, cellSize, AssetLabelHeight);
            EditorGUI.TextField(nameRect, "", assetName);
            if (assetPath.Equals(_selectAssetPath, StringComparison.Ordinal))
            {
                BookMarkGuiUtil.DrawRectOutline(drawRect, Color.green);
            }
        }

        private void HandleDrawRectInput(Rect drawRect, string assetPath)
        {
            
            if (Event.current.button == 0)
            {
                if (Event.current.isMouse && drawRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown )
                    {
                        mouseDownAsset = assetPath;
                        Event.current.Use();
                   
                    }else if (Event.current.type == EventType.MouseDrag)
                    {
                        if (assetPath.Equals(mouseDownAsset, StringComparison.Ordinal))
                        {
                            DragDropUtil.SetupDragAsset(assetPath);
                        }
                    }
                    else if (Event.current.type == EventType.MouseUp  && assetPath == mouseDownAsset)
                    {
                        HandleClickAsset(assetPath);
                        Event.current.Use();
                    } 
                }
                if (Event.current.type == EventType.DragPerform && drawRect.Contains(Event.current.mousePosition))
                {
                    if (assetPath.Equals(mouseDownAsset, StringComparison.Ordinal)==false)
                    {
                        Event.current.Use();
                        DragDropUtil.TryReplaceAsset(assetPath);
                    }
                }
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
            }

            if (Event.current.button == 1 )
            {
                if (Event.current.type == EventType.MouseDown && drawRect.Contains(Event.current.mousePosition))
                {
                    mouseDownAsset = assetPath;
                }

                if (Event.current.type == EventType.MouseUp)
                {
                    if (drawRect.Contains(Event.current.mousePosition) && assetPath.Equals(mouseDownAsset, StringComparison.Ordinal))
                    {
                        var rect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 100, 20);
                        var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                        EditorUtility.DisplayPopupMenu( rect,"KyleKit/Assets/AssetContext", new MenuCommand(obj));
                        Event.current.Use();
                    }
                    else
                    {
                        
                    }
                }
            }
        }

        private void HandleClickAsset(string assetPath)
        {
            _selectAssetPath = assetPath;
            if (ItemClickDelegate != null)
            {
                ItemClickDelegate.Invoke(assetPath);
            }
        }


        bool IsVisible(Rect drawRect)
         {
             if (drawRect.y + heightGap + cellSize >= scroll_pos.y && drawRect.y < scroll_pos.y + _drawRect.height) 
             {
                 return true;
             }
             return false;
         }

    }
}