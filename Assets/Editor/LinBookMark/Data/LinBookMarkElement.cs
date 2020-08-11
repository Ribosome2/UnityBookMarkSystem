﻿using UnityEditor;
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
            if (type == BookMarkType.AssetFolder || type == BookMarkType.SingleAsset)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(AssetGuild);
                var iconTex =(Texture2D) AssetDatabase.GetCachedIcon(assetPath);
                if (iconTex==null)
                {
                    iconTex= (Texture2D) EditorGUIUtility.Load(("CollabError"));
                }

                return iconTex;
            }
            return (Texture2D) EditorGUIUtility.Load(("d_EditCollider"));
        }
    }
}