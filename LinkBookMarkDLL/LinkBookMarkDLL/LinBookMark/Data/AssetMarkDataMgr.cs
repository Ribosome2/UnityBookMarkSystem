using System.Collections.Generic;

namespace LinBookMark
{
    public static  class AssetMarkDataMgr
    {
        private const string AssetMarkFileName = "AssectMark";
        public static Dictionary<string,string> AssetsMarkDict = new Dictionary<string, string>();
        public static void  Init()
        {
            AssetsMarkDict.Clear();
            var assetMark = DataSaver.ReadFromDisk<AssetMarkData>(AssetMarkFileName);
            if (assetMark != null)
            {
                foreach (var markRecord in assetMark.AssetsMarkList)
                {
                    AssetsMarkDict[markRecord.AssetPath] = markRecord.Icon;
                }
            }
        }

        public static void Save()
        {
            AssetMarkData data = new AssetMarkData();
            var marks = new List<AssetMarkRecord>();
            var iter = AssetsMarkDict.GetEnumerator();
            while (iter.MoveNext())
            {
                var currentItem = iter.Current.Value;
                marks.Add(new AssetMarkRecord(){AssetPath =  iter.Current.Key,Icon = iter.Current.Value});
                
            }
            iter.Dispose();
            data.AssetsMarkList = marks;
            DataSaver.WriteToDisk(AssetMarkFileName,data);
        }
    }
}