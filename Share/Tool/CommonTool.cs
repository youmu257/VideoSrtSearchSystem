namespace Share.Tool
{
    public class CommonTool : ICommonTool
    {
        /// <summary>
        /// 計算總頁數
        /// </summary>
        public int GetTotalPage(int totalCount, int pageSize)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        /// <summary>
        /// 檢查時間否在指定的範圍內
        /// </summary>
        /// <param name="time">要確認的時間</param>
        public bool CheckInTimeRage(DateTime time, string start, string end, string format = "yyyy-MM-dd")
        {
            // 如果開始時間或結束時間為空，視為不限制
            if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
            {
                return true;
            }

            try
            {
                // 檢查開始時間
                if (!string.IsNullOrEmpty(start))
                {
                    DateTime startTime = DateTime.ParseExact(start, format, System.Globalization.CultureInfo.InvariantCulture);
                    if (time < startTime)
                    {
                        return false;
                    }
                }

                // 檢查結束時間
                if (!string.IsNullOrEmpty(end))
                {
                    DateTime endTime = DateTime.ParseExact(end, format, System.Globalization.CultureInfo.InvariantCulture);
                    if (time > endTime)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (FormatException ex)
            {
                // 時間格式解析錯誤
                throw new ArgumentException($"無法解析時間字串，請確保格式為 {format}", ex);
            }
        }

        /// <summary>
        /// 檢查日期格式是否為 yyyy-MM-dd
        /// </summary>
        /// <param name="dateString">要檢查的日期字串</param>
        /// <returns>日期格式是否有效</returns>
        public bool IsValidDateFormat(string dateString, string dateFormat = "yyyy-MM-dd")
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return true;
            }
            // 嘗試將字串轉換為 DateTime，使用 yyyy-MM-dd 格式
            return DateTime.TryParseExact(
                dateString,
                dateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _);
        }
    }
}
