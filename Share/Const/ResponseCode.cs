namespace Share.Const
{
    /// <summary>
    /// 0XXX: 成功系列
    /// 2XXX: 通用系列
    /// 3XXX: 標籤系列
    /// 9XXX: 通用錯誤系列
    /// </summary>
    public class ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string SUCCESS = "0000";

        /// <summary>
        /// 必填欄位未填寫
        /// </summary>
        public const string PARAMETER_IS_REQUIRED = "2001";

        /// <summary>
        /// 不合法的欄位
        /// </summary>
        public const string PARAMETER_FORMAT_ERROR = "2002";

        /// <summary>
        /// 找不到檔案
        /// </summary>
        public const string FILE_NOT_FOUND = "2003";

        /// <summary>
        /// 標籤類型不存在
        /// </summary>
        public const string TAG_TYPE_NOT_EXIST = "3001";

        /// <summary>
        /// 標籤已存在
        /// </summary>
        public const string TAG_IS_EXIST = "3002";

        /// <summary>
        /// 通用失敗回傳
        /// </summary>
        public const string FAIL = "9999";
    }
}
