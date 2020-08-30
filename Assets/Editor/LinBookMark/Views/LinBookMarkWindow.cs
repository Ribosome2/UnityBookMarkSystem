using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            window.minSize = new Vector2(230, 250);
            window.Show();
        }

  
        [SerializeField] TreeViewState m_TreeViewState;

        // The TreeView is not serializable it should be reconstructed from the tree data.
        LinBookMarkTreeView m_TreeView;
        SearchField m_SearchField;
        public GUIContent m_CreateDropdownContent = new GUIContent("Create");
        private BookMarkGUIStyles guiStyles;
        private WindowSplitterDrawer splitter ;
        public FolderAssetListView assetListView;
        void OnEnable()
        {
            splitter = new WindowSplitterDrawer(this,this);
            CheckInitState();


            m_TreeView = new LinBookMarkTreeView(m_TreeViewState);
            m_TreeView.OnSelectionChange += OnTreeSelectionChange;
            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;
            BookMarkDataCenter.instance.BookMarkDataChangeEvent += m_TreeView.Reload;
            EditorApplication.projectWindowChanged += new EditorApplication.CallbackFunction(this.OnProjectChanged);
            AssemblyReloadEvents.afterAssemblyReload += new AssemblyReloadEvents.AssemblyReloadCallback(this.OnAfterAssemblyReload);
            
            OnTreeSelectionChange(m_TreeView.GetSelection());
        }

        private void CheckInitState()
        {
            // Check if we already had a serialized view state (state 
            // that survived assembly reloading)
            if (m_TreeViewState == null)
            {
                m_TreeViewState = new TreeViewState();
                Debug.Log("new treedd");
            }

            if (assetListView == null)
            {
                assetListView = new FolderAssetListView();
            }
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
            RefreshAssetList();
        }

        private void RefreshAssetList()
        {
            var treeItemIds = m_TreeView.GetSelection();
            List<string> asstPathList = new List<string>();
            foreach (var id in treeItemIds)
            {
                var element = BookMarkDataCenter.instance.bookMarkDataModel.Find(id);
                if (element != null && string.IsNullOrEmpty(element.AssetGuild) == false)
                {
                    var projectPath = AssetDatabase.GUIDToAssetPath(element.AssetGuild);
                    if (string.IsNullOrEmpty(projectPath) == false)
                    {
                        asstPathList.Add(projectPath);
                    }
                }
            }

            List<string> expandList = BookMarkDataCenter.instance.GetMainPathsOfAssetsFromAutoExpandNodes(treeItemIds);
            asstPathList.AddRange(expandList);
            assetListView.SetAssetList(asstPathList);
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

            if (assetListView != null)
            {
                assetListView.RefreshAssetList();
            }
        }


        private void OnProjectChanged()
        {
            RebuildTreeView();
            RefreshAssetList();
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
            assetListView.OnGUI(splitter.ListAreaRect,splitter.GridSize);
            splitter.BreadCrumbBar(assetListView.GetParentFolderDesc(),guiStyles);
            HandleCommandEvents();
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
            return  18f ;
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
                    // Debug.Log(Event.current.commandName);
                    if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
                    {
                        HandleDelete(flag);
                    }else if (Event.current.commandName == "Copy")
                    {
                        HandleCopy();
                    }else if (Event.current.commandName == "Paste")
                    {
                        HandlePaste();
                    }

                    break;
            }

            return false;
        }

        private void HandleCopy()
        {
            var selectedIds = m_TreeView.GetSelection();
            StringBuilder sb = new StringBuilder();
            foreach (var treeId in selectedIds)
            {
                var assetPath = BookMarkDataCenter.instance.GetAssetPath(treeId);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(assetPath);
                }
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
        }

        private void HandlePaste()
        {
            var selectedIds = m_TreeView.GetSelection();
            if (selectedIds.Count == 1)
            {
                var folderPath = BookMarkDataCenter.instance.GetAssetPath(selectedIds[0]);
                FileUtil.CheckPasteFiles(folderPath);
            }
        }

        private void HandleDelete(bool flag)
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
    }
}