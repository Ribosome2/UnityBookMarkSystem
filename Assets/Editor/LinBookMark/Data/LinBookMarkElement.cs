using UnityEditor;
using UnityEditor.TreeViewExamples;
using UnityEngine;

namespace LinBookMark
{
    public enum BookMarkType
    {
        None=0,
        CustomRoot = 1,
        AssetFolder =2,
        SingleAsset =3,
    }
    [System.Serializable]
    public class LinBookMarkElement:TreeElement
    {
        public BookMarkType type = BookMarkType.CustomRoot;
        public string AssetGuild = string.Empty;

        public virtual Texture2D GetIcon()
        {
            return (Texture2D) EditorGUIUtility.Load((EditorGUIUtility.isProSkin ?"d_EditCollider": "EditCollider"));
        }

        public string GetProjectPath()
        {
            return  AssetDatabase.GUIDToAssetPath(AssetGuild);
        }
    }
}