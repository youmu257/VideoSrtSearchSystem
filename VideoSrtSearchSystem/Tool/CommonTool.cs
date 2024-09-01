namespace VideoSrtSearchSystem.Tool
{
    public class CommonTool : ICommonTool
    {
        public int GetTotalPage(int totalCount, int pageSize)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }
}
