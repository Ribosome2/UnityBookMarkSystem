using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public class BookMarkCreateCommand
    {
        [MenuItem("KyleKit/Create/Group", false, 2100)]
        public static void CreateBookMarkGroup(MenuCommand menuCommand)
        {
            BookMarkDataCenter.instance.CreateOneBookMarkItem();
        }

         [MenuItem("KyleKit/Create/Group", true)]
        static bool ValidateCreateBookMarkGroup()
        {
             LinBookMarkWindow[] windows = Resources.FindObjectsOfTypeAll<LinBookMarkWindow>();
            if(windows != null && windows.Length > 0)
            {
                return true;
            }
            return false;
        }
    }
}