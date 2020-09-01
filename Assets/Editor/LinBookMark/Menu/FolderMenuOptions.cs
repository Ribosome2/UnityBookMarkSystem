using LinBookMark;
using UnityEditor;

namespace LinBookMark
{
    public  class FolderMenuOptions
    {
        [MenuItem("KyleKit/FolderMark/SpriteFolder ")]
        public static void MarkAsSpriteFolder(string guild)
        {
            if (string.IsNullOrEmpty(guild) == false)
            {
                BookMarkDataCenter.instance.FoldersMark[guild] = "Sprite Icon";
            }
        }
    }
}