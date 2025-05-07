namespace Share.Tool
{
    public interface ICommonTool
    {
        int GetTotalPage(int totalCount, int pageSize);
        bool CheckInTimeRage(DateTime time, string start, string end, string format = "yyyy-MM-dd");
        bool IsValidDateFormat(string dateString, string dateFormat = "yyyy-MM-dd");
    }
}
