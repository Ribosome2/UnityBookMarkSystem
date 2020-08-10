namespace LinBookMark
{
    public static class TreeItemIdGenerator
    {
        private static int autoId = 1;

        public static int NextId
        {
            get
            { 
                return autoId++;
            }
        }

        public static void ResetId()
        {
            autoId = 1;
        }
    }
}