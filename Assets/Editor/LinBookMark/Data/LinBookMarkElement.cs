using UnityEditor.TreeViewExamples;

namespace LinBookMark
{
    public enum BookMarkType
    {
        CustomRoot = 1,
        AssetFolder =2,
        SingleAsset =3,
    }
    public class LinBookMarkElement:TreeElement
    {

        public BookMarkType type;
    }
}