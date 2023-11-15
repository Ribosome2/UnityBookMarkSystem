using System;
using System.Collections.Generic;
using System.IO;

namespace LinBookMark
{
    public class AssetPathFilterByFileName:IAssetPathFilter
    {
        public IList<string> GetFilterPaths(IList<string> pathList, string match)
        {
            if (string.IsNullOrEmpty(match)) return pathList;

            var list = new List<string>();

            // First try to find an exact match
            for (int i = 0, imax = pathList.Count; i < imax; ++i)
            {
                var s = pathList[i];
                var fileName = Path.GetFileName(s);

                if (s != null && !string.IsNullOrEmpty(fileName) && string.Equals(match, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(s);
                    return list;
                }
            }

            // No exact match found? Split up the search into space-separated components.
            string[] keywords = match.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < keywords.Length; ++i) keywords[i] = keywords[i].ToLower();

            // Try to find all paths where all keywords are present
            for (int i = 0, imax = pathList.Count; i < imax; ++i)
            {
                var fileName = Path.GetFileName(pathList[i]);
                if (fileName != null && !string.IsNullOrEmpty(fileName))
                {
                    string tl = fileName.ToLower();
                    int matches = 0;

                    for (int b = 0; b < keywords.Length; ++b)
                    {
                        if (tl.Contains(keywords[b])) ++matches;
                    }
                    if (matches == keywords.Length) list.Add(pathList[i]);
                }
            }
            return list;
        }

    }
}