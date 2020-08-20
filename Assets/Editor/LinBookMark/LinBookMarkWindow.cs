﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
namespace LinBookMark
{
    public class LinBookMarkWindow : EditorWindow
    {
        [MenuItem("KyleKit/LinBookMarkd %k")]
        private static void ShowWindow()
        {
            var window = GetWindow<LinBookMarkWindow>();
            window.titleContent = new GUIContent("BookMark", (Texture2D) EditorGUIUtility.Load(("FilterByLabel")));
            window.position = new Rect(0,100f,500f,500f);
            window.Show();
        }

        // We are using SerializeField here to make sure view state is written to the window 
        // layout file. This means that the state survives restarting Unity as long as the window
        // is not closed. If omitting the attribute then the state just survives assembly reloading 
        // (i.e. it still gets serialized/deserialized)
        [SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        LinBookMarkTreeView m_TreeView;
        SearchField m_SearchField;
        public GUIContent m_CreateDropdownContent = new GUIContent("Create");
        private BookMarkGUIStyles guiStyles;
        void OnEnable()
        {
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_TreeView = new LinBookMarkTreeView(m_TreeViewState);
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            BookMarkDataCenter.instance.BookMarkDataChangeEvent += m_TreeView.Reload;
            EditorApplication.projectWindowChanged += new EditorApplication.CallbackFunction(this.OnProjectChanged);
            AssemblyReloadEvents.afterAssemblyReload += new AssemblyReloadEvents.AssemblyReloadCallback(this.OnAfterAssemblyReload);

        }

        private void OnAfterAssemblyReload()
        {
            RebuildTreeView();
        }

        private void RebuildTreeView()
        {
            if (m_TreeView != null)
            {
                m_TreeView.Reload();
            }
        }


        private void OnProjectChanged()
        {
            RebuildTreeView();
            
        }


        void OnGUI()
        {
            if (guiStyles == null)
            {
                guiStyles= new BookMarkGUIStyles();
            }
            
            DoToolbar();
            DoTreeView();
            CalculateRects();
            this.ResizeHandling(this.position.width, this.position.height - this.m_ToolbarHeight);
            BookMarkGuiUtil.DrawHorizontalSplitter(new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, 1f, this.m_TreeViewRect.height));
            this.BottomBar();
            HandleCommandEvents();
            
        }

        private float m_DirectoriesAreaWidth = 115f;
        private const float k_MinHeight = 250f;
        private const float k_MinWidthOneColumn = 230f;
        private const float k_MinWidthTwoColumns = 230f;
        private float m_ToolbarHeight=18f;
        private const float k_BottomBarHeight = 17f;
        private const float k_ResizerWidth = 5f;
        private const float k_SliderWidth = 55f;
        [NonSerialized]
        private Rect m_ListAreaRect;
        [NonSerialized]
        private Rect m_TreeViewRect;
        [NonSerialized]
        private Rect m_BottomBarRect;
        [NonSerialized]
        private Rect m_ListHeaderRect;
        private float k_MinDirectoriesAreaWidth = 110f;
        [NonSerialized]
        private float m_SearchAreaMenuOffset = -1f;
        private string m_SelectedPath;
        private float m_LastListWidth;
        private List<GUIContent> m_SelectedPathSplitted = new List<GUIContent>();
        private string m_AssetStoreError = "";
        private int m_MinIconSize = 32;
        private int gridSize = 20;
        private int m_MinGridSize = 16;
        private int m_MaxGridSize = 96;
        
        private void ResizeHandling(float width, float height)
        {
           
            Rect dragRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight, 5f, height);
            dragRect = BookMarkGuiUtil.HandleHorizontalSplitter(dragRect, this.position.width, this.k_MinDirectoriesAreaWidth, 230f - this.k_MinDirectoriesAreaWidth);
            this.m_DirectoriesAreaWidth = dragRect.x;
            float num = this.position.width - this.m_DirectoriesAreaWidth;
            if ((double) num != (double) this.m_LastListWidth)
            {
                this.RefreshSplittedSelectedPath();
            }
            this.m_LastListWidth = num;
        }
        private float GetBottomBarHeight()
        {
            if (this.m_SelectedPathSplitted.Count == 0)
                this.RefreshSplittedSelectedPath();
            return   17f * (float) this.m_SelectedPathSplitted.Count;
        }

        private float GetListHeaderHeight()
        {
            return this.m_SearchField.HasFocus() ? 18f : 0.0f;
        }
        private void CalculateRects()
        {
            float bottomBarHeight = this.GetBottomBarHeight();
            float listHeaderHeight = this.GetListHeaderHeight();
           

            float width = this.position.width - this.m_DirectoriesAreaWidth;
            this.m_ListAreaRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight + listHeaderHeight, width, this.position.height - this.m_ToolbarHeight - listHeaderHeight - bottomBarHeight);
            this.m_TreeViewRect = new Rect(0.0f, 18, this.m_DirectoriesAreaWidth, this.position.height - this.m_ToolbarHeight);
            this.m_BottomBarRect = new Rect(this.m_DirectoriesAreaWidth, this.position.height - bottomBarHeight, width, bottomBarHeight);
            this.m_ListHeaderRect = new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, this.m_ListAreaRect.width, listHeaderHeight);
        }
        
        private void BottomBar()
        {
            if ((double) this.m_BottomBarRect.height == 0.0)
                return;
            Rect bottomBarRect = this.m_BottomBarRect;
            GUI.Label(bottomBarRect, GUIContent.none, guiStyles.bottomBarBg);
            this.IconSizeSlider(new Rect((float) ((double) bottomBarRect.x + (double) bottomBarRect.width - 55.0 - 16.0), (float) ((double) bottomBarRect.y + (double) bottomBarRect.height - 17.0), 55f, 17f));
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

        private void RefreshSplittedSelectedPath()
        {
     
            this.m_SelectedPathSplitted.Clear();
            if (string.IsNullOrEmpty(this.m_SelectedPath))
            {
                this.m_SelectedPathSplitted.Add(new GUIContent());
            }
            else
            {
                string str1 = this.m_SelectedPath;
                if (this.m_SelectedPath.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
                    str1 = this.m_SelectedPath.Substring("assets/".Length);
              
                {
                    float num = (float) ((double) this.position.width - (double) this.m_DirectoriesAreaWidth - 55.0 - 16.0);
                    if ((double) GUI.skin.label.CalcSize(new GUIContent(str1)).x + 25.0 > (double) num)
                    {
                        string[] strArray = str1.Split('/');
                        string str2 = "Assets/";
                        for (int index = 0; index < strArray.Length; ++index)
                        {
                            string path = str2 + strArray[index];
                            Texture cachedIcon = AssetDatabase.GetCachedIcon(path);
                            this.m_SelectedPathSplitted.Add(new GUIContent(strArray[index], cachedIcon));
                            str2 = path + "/";
                        }
                    }
                    else
                        this.m_SelectedPathSplitted.Add(new GUIContent(str1, AssetDatabase.GetCachedIcon(this.m_SelectedPath)));
                }
            }
        }
        
        
        void DoToolbar()
        {

            if (m_TreeView != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                this.CreateDropdown();
                GUILayout.Space(100);
                GUILayout.FlexibleSpace();

                m_TreeView.searchString = m_SearchField.OnToolbarGUI(m_TreeView.searchString);
                GUILayout.EndHorizontal();
            }
        }

        private void CreateDropdown()
        {
            Rect rect = GUILayoutUtility.GetRect(m_CreateDropdownContent, EditorStyles.toolbarDropDown);
            if (!EditorGUI.DropdownButton(rect, m_CreateDropdownContent, FocusType.Passive,
                EditorStyles.toolbarDropDown))
                return;
            GUIUtility.hotControl = 0;
            EditorUtility.DisplayPopupMenu(rect, "KyleKit/Create", (MenuCommand) null);
        }

        void DoTreeView()
        {
            if (m_TreeView != null)
            {
                m_TreeView.OnGUI(m_TreeViewRect);
            }
        }

        private bool HandleCommandEvents()
        {
            UnityEngine.EventType type = Event.current.type;
            switch (type)
            {
                case UnityEngine.EventType.ValidateCommand:
                case UnityEngine.EventType.ExecuteCommand:
                    bool flag = type == UnityEngine.EventType.ExecuteCommand;
                    if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
                    {
                        Event.current.Use();
                        if (flag)
                        {
                            bool askIfSure = Event.current.commandName == "SoftDelete";
                            if (AssetOperationUtil.DeleteAssets(m_TreeView.GetSelection(), askIfSure))
                            {
                                m_TreeView.Reload();
                            }
                            if (askIfSure)
                                this.Focus();
                        }
                        GUIUtility.ExitGUI();
                    }

                    break;
            }

            return false;
        }
    }
}