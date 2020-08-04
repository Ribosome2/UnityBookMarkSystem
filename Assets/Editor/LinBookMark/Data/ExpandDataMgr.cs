using System.Collections.Generic;

namespace LinBookMark
{
    public class ExpandDataMgr
    {
        Dictionary<int ,ExpandData> expandDataMap = new Dictionary<int, ExpandData>();
        
        public void ClearExpandDataMap()
        {
            expandDataMap.Clear();
        }
        public void SetExpandData(int treeItemId, ExpandData data)
        {
            expandDataMap[treeItemId] = data;
        }

        public ExpandData GetExpandData(int treeItemId)
        {
            ExpandData result;
            expandDataMap.TryGetValue(treeItemId, out result);
            return result;
        }
    }
}