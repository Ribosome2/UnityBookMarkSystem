using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
namespace LinBookMark
{
    public class LinBookMarkWindow : EditorWindow,ISplittableWindow
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
        private WindowSplitterDrawer splitter ;
        public FolderAssetListView  assetListView = new FolderAssetListView();
        void OnEnable()
        {
            splitter = new WindowSplitterDrawer(this,this);
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            m_TreeView = new LinBookMarkTreeView(m_TreeViewState);
            m_TreeView.OnSelectionChange += OnTreeSelectionChange;
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            BookMarkDataCenter.instance.BookMarkDataChangeEvent += m_TreeView.Reload;
            EditorApplication.projectWindowChanged += new EditorApplication.CallbackFunction(this.OnProjectChanged);
            AssemblyReloadEvents.afterAssemblyReload += new AssemblyReloadEvents.AssemblyReloadCallback(this.OnAfterAssemblyReload);
            
            OnTreeSelectionChange(m_TreeView.GetSelection());
        }

     

        private void OnDisable()
        {
            m_SearchField.downOrUpArrowKeyPressed -= m_TreeView.SetFocusAndEnsureSelectedItem;
            BookMarkDataCenter.instance.BookMarkDataChangeEvent += m_TreeView.Reload;
            EditorApplication.projectWindowChanged -= new EditorApplication.CallbackFunction(this.OnProjectChanged);
            AssemblyReloadEvents.afterAssemblyReload -= new AssemblyReloadEvents.AssemblyReloadCallback(this.OnAfterAssemblyReload);

            m_TreeView.OnSelectionChange -= OnTreeSelectionChange;

        }

        private void OnTreeSelectionChange(IList<int> list)
        {
            List<string> folderList = new List<string>();
            foreach (var id in list)
            {
                var element = BookMarkDataCenter.instance.bookMarkDataModel.Find(id);
                if (element != null && string.IsNullOrEmpty(element.AssetGuild)==false)
                {
                    var projectPath = AssetDatabase.GUIDToAssetPath(element.AssetGuild);
                    if (string.IsNullOrEmpty(projectPath) == false)
                    {
                        folderList.Add(projectPath);
                    }
                }
            }
            assetListView.SetFolderList(folderList);
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
            splitter.OnGUI(guiStyles);
            HandleCommandEvents();
            assetListView.OnGUI(splitter.ListAreaRect);
        }


        [NonSerialized]
        private float m_SearchAreaMenuOffset = -1f;
        private string m_SelectedPath;
        private List<GUIContent> m_SelectedPathSplitted = new List<GUIContent>();
        private string m_AssetStoreError = "";
  

        public float GetBottomBarHeight()
        {
            if (this.m_SelectedPathSplitted.Count == 0)
                this.RefreshSplittedSelectedPath();
            return   17f * (float) this.m_SelectedPathSplitted.Count;
        }

        public float GetListHeaderHeight()
        {
            return this.m_SearchField.HasFocus() ? 18f : 0.0f;
        }


        private void RefreshSelectedPath()
        {
            this.m_SelectedPath = !(Selection.activeObject != (UnityEngine.Object) null) ? "" : AssetDatabase.GetAssetPath(Selection.activeObject);
            this.m_SelectedPathSplitted.Clear();
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
                    float num = (float) ((double) this.position.width - (double) splitter.m_DirectoriesAreaWidth - 55.0 - 16.0);
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
                m_TreeView.OnGUI(splitter.m_TreeViewRect);
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