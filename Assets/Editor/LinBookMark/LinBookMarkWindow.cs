﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
namespace LinBookMark
{
    public class LinBookMarkWindow : EditorWindow
    {
        [MenuItem("KyleKit/LinBookMark %k")]
        private static void ShowWindow()
        {
            var window = GetWindow<LinBookMarkWindow>();
            window.titleContent = new GUIContent("BookMark", (Texture2D) EditorGUIUtility.Load(("FilterByLabel")));
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
            DoToolbar();
            DoTreeView();
            HandleCommandEvents();
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
                Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
                m_TreeView.OnGUI(rect);
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