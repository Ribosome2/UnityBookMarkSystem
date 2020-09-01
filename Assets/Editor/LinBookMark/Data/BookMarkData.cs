using System.Collections.Generic;

namespace LinBookMark
{
    [System.Serializable]
    public class BookMarkData
    {
        public List<LinBookMarkElement> bookMarks = new List<LinBookMarkElement>();
        public Dictionary<string,string> foldersMark = new Dictionary<string, string>();
    }
}