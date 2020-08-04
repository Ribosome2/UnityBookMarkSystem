using UnityEditor.TreeViewExamples;

namespace LinBookMark
{
    public enum BookMarkType
    {
        None=0,
        CustomRoot = 1,
        AssetFolder =2,
        SingleAsset =3,
    }
    public class LinBookMarkElement:TreeElement
    {

        public BookMarkType type = BookMarkType.CustomRoot;
        public int ExtraId;
        public string AssetGuild = string.Empty;
    }
}