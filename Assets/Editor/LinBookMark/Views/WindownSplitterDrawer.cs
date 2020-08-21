using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class WindowSplitterDrawer
    {
        private EditorWindow _window;
        private ISplittableWindow _splittableWindow;
        public WindowSplitterDrawer(EditorWindow window,ISplittableWindow splittableWindow)
        {
            _window = window;
            _splittableWindow = splittableWindow;
        }
        
        private float m_LastListWidth;
        private float k_MinDirectoriesAreaWidth = 110f;
        public float m_DirectoriesAreaWidth = 115f;
        private float m_ToolbarHeight=18f;
        public Action onSplitSizeChange;
        
        [NonSerialized]
        public Rect ListAreaRect;
        [NonSerialized] public Rect m_TreeViewRect;
        [NonSerialized]
        private Rect m_BottomBarRect;
        [NonSerialized]
        private Rect m_ListHeaderRect;
        private List<GUIContent> m_SelectedPathSplitted = new List<GUIContent>();
        private int gridSize = 20;
        private int m_MinGridSize = 16;
        private int m_MaxGridSize = 96;
    
        public void OnGUI(BookMarkGUIStyles guiStyles)
        {
            CalculateRects();
            ResizeHandling(_window,_window.position.height - this.m_ToolbarHeight);
            BookMarkGuiUtil.DrawHorizontalSplitter(new Rect(this.ListAreaRect.x, this.m_ToolbarHeight, 1f, this.m_TreeViewRect.height));
            BottomBar(guiStyles);
        }
        
        public void ResizeHandling(EditorWindow window, float height)
        {
       
            Rect dragRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight, 5f, height);
            dragRect = BookMarkGuiUtil.HandleHorizontalSplitter(dragRect, window.position.width, this.k_MinDirectoriesAreaWidth, 230f - this.k_MinDirectoriesAreaWidth);
            this.m_DirectoriesAreaWidth = dragRect.x;
            float num = window.position.width - this.m_DirectoriesAreaWidth;
            if ((double) num != (double) this.m_LastListWidth)
            {
                if (onSplitSizeChange != null)
                {
                    onSplitSizeChange();
                }
            }
            this.m_LastListWidth = num;
        }
        
        private void CalculateRects()
        {
            float bottomBarHeight = _splittableWindow.GetBottomBarHeight();
            float listHeaderHeight = _splittableWindow.GetListHeaderHeight();
           

            float width = _window.position.width - this.m_DirectoriesAreaWidth;
            this.ListAreaRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight + listHeaderHeight, width, _window.position.height - this.m_ToolbarHeight - listHeaderHeight - bottomBarHeight);
            this.m_TreeViewRect = new Rect(0.0f, 18, this.m_DirectoriesAreaWidth, _window.position.height - this.m_ToolbarHeight);
            this.m_BottomBarRect = new Rect(this.m_DirectoriesAreaWidth, _window.position.height - bottomBarHeight, width, bottomBarHeight);
            this.m_ListHeaderRect = new Rect(this.ListAreaRect.x, this.m_ToolbarHeight, this.ListAreaRect.width, listHeaderHeight);
        }
        
        private void BottomBar(BookMarkGUIStyles guiStyles)
        {
            if ((double) this.m_BottomBarRect.height == 0.0)
                return;
            Rect bottomBarRect = this.m_BottomBarRect;
            GUI.Label(bottomBarRect, GUIContent.none, guiStyles.bottomBarBg);
            var xPos = (float) ((double) bottomBarRect.x + (double) bottomBarRect.width - 55.0 - 16.0);
            var yPos = (float) ((double) bottomBarRect.y + (double) bottomBarRect.height - 17.0);
            this.IconSizeSlider(new Rect(xPos, yPos, 55f, 17f));
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            bottomBarRect.width -= 4f;
            bottomBarRect.x += 2f;
            bottomBarRect.height = 17f;
            for (int index = this.m_SelectedPathSplitted.Count - 1; index >= 0; --index)
            {
                if (index == 0)
                    bottomBarRect.width = (float) ((double) bottomBarRect.width - 55.0 - 14.0);
                GUI.Label(bottomBarRect, this.m_SelectedPathSplitted[index], guiStyles.selectedPathLabel);
                bottomBarRect.y += 17f;
            }
            EditorGUIUtility.SetIconSize(new Vector2(0.0f, 0.0f));
        }
        
        private void IconSizeSlider(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            int num = (int) GUI.HorizontalSlider(r, (float) gridSize, (float) m_MinGridSize, (float) m_MaxGridSize);
            if (!EditorGUI.EndChangeCheck())
                return;
            this.gridSize = num;
        }
        

    }
}