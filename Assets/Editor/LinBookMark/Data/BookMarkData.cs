using System;
using System.Collections.Generic;

namespace LinBookMark
{
    [System.Serializable]
    public class BookMarkData
    {
        public List<LinBookMarkElement> bookMarks = new List<LinBookMarkElement>();
    }

    [Serializable]
    public class AssetMarkRecord
    {
        public string AssetPath;
        public string Icon;
    }
    
    [System.Serializable]
    public class AssetMarkData
    {
        public List<AssetMarkRecord> AssetsMarkList = new List<AssetMarkRecord>();
    }
    
}