using System.Collections.Generic;

namespace LinBookMark
{
    public interface IAssetPathFilter
    {
        IList<string> GetFilterPaths(IList<string> pathList, string match);
    }
}