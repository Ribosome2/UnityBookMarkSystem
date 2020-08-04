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
        
        public void  Init()
        {
            InitBookMarkList();
            bookMarkDataModel = new TreeModel<LinBookMarkElement>(bookMarks);
        }
        
        
        private void InitBookMarkList()
        {
            var bookMark = new LinBookMarkElement() {name = "Root", depth = -1, id = TreeItemIdGenerator.NextId};
            bookMarks.Add(bookMark);
            var bookM2 = new LinBookMarkElement() {name = "customChild1", depth = 0, id = TreeItemIdGenerator.NextId};
            bookMarks.Add(bookM2);
            bookMarks.Add(new LinBookMarkElement() {name = "Kyle ", depth = 1, id = TreeItemIdGenerator.NextId});
        }



        
        
        public List<UnityObject> GetUnityObjectList(IList<int> sortedDraggedIDs)
        {
            List<UnityObject> objList = new List<UnityObject>(sortedDraggedIDs.Count);
            foreach (var id in sortedDraggedIDs)
            {
                Debug.Log("drag "+id );
                UnityObject obj = EditorUtility.InstanceIDToObject(id);
                if (obj != null)
                    objList.Add(obj);
            }

            return objList;
        }
    }
}