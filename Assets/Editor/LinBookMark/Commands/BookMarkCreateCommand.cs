using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkCreateCommand
    {
        [MenuItem("LinBookMark/Create/Group", false, 2100)]
        public static void CreateBookMarkGroup(MenuCommand menuCommand)
        {
            BookMarkDataCenter.instance.CreateOneBookMarkItem();
        }
        
      
    }
}