using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace LinBookMark
{
    public class BookMarkDataCenter
    {
        private static BookMarkDataCenter _instance;

        public static BookMarkDataCenter instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance= new BookMarkDataCenter();
                }

                return _instance;
            }
        }
        
        List<LinBookMarkElement> bookMarks = new List<LinBookMarkElement>();
        public TreeModel<LinBookMarkElement> bookMarkDataModel;
        public ExpandDataMgr ExpandDataMgr = new ExpandDataMgr();
        public Action BookMarkDataChangeEvent ;
        public void  Init()
        {
            InitBookMarkList();
            ConvertBookMarkListToTreeModel();
        }

        void ConvertBookMarkListToTreeModel()
        {
            bookMarkDataModel = new TreeModel<LinBookMarkElement>(bookMarks);
        }
        
        
        private void InitBookMarkList()
        {
            bookMarks.Clear();
            var bookMark = new LinBookMarkElement() {name = "Root", depth = -1, id = TreeItemIdGenerator.NextId};
            bookMarks.Add(bookMark);
            var saveList = DataSaver.ReadFromDisk<List<LinBookMarkElement>>(DataSaver.DataFileName);
            if (saveList != null)
            {
                for (int i = 0; i < saveList.Count; i++)
                {
                    var item = saveList[i];
                    bookMarks.Add(bookMark);
                }
            }
        }

        public void SaveCurrentTreeModel()
        {
            List<LinBookMarkElement> allList = new List<LinBookMarkElement>();
            TreeElementUtility.TreeToList(bookMarkDataModel.root, allList);
            List<LinBookMarkElement> listToSave = new List<LinBookMarkElement>();
            for (int i = 0; i < allList.Count; i++)
            {
                var element = allList[i];
                if (element.depth > 0)
                {
                    listToSave.Add(element);
                }
            }
            DataSaver.WriteToDisk(DataSaver.DataFileName,listToSave);
        }
        

        public void CreateOneBookMarkItem()
        {
            if (bookMarks.Count > 0)
            {
                var bookM2 = new LinBookMarkElement() {name = "BookMarkGroup", depth = 0, id = TreeItemIdGenerator.NextId};
                bookMarks.Add(bookM2);
                ConvertBookMarkListToTreeModel();
                if (BookMarkDataChangeEvent!=null)
                {
                    BookMarkDataChangeEvent.Invoke();
                }

                SaveCurrentTreeModel();
            }
        }
        

    }
}