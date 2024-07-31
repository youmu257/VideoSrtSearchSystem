using System.Runtime.Serialization;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.Tool.Language;

namespace VideoSrtSearchSystem.Exceptions
{
    public class MyException : Exception, ISerializable
    {
        public string Code = ResponseCode.FAIL;
        public new string Message = "Error";

        public MyException() : base("show message") { }

        public MyException(string code, string message = "") : base(message)
        {
            Code = code;
            if (string.IsNullOrEmpty(message))
            {
                message = LangTool.GetTranslation(code);
            }
            if (string.IsNullOrEmpty(message))
            {
                message = "Error";
            }
            Message = message;
        }

        public MyException(string code, Exception inner) : base(code, inner)
        {
            Code = code;
        }
    }
}
