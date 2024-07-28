using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using VideoSrtSearchSystem.Config;
using VideoSrtSearchSystem.Tool.Language;

namespace VideoSrtSearchSystem.Controllers
{
    public class BaseController<T>(ILogger<T> _logger) : ControllerBase
    {
        [NonAction]
        public void SetControllerContext(ControllerContext context)
        {
            ControllerContext = context;
        }

        protected ObjectResult ParameterIsRequired()
        {
            string code = ResponseCode.PARAMETER_IS_REQUIRED;
            var stackTrace = new StackTrace();
            string message = LangTool.GetTranslation("common_parameter_is_required");
            LogResponse(stackTrace, code, message);
            return UnprocessableEntity(new
            {
                code,
                message
            });
        }

        protected ObjectResult ParameterIsRequired(string parameterName, string code = "4000")
        {
            var stackTrace = new StackTrace();
            string message = LangTool.GetTranslation("common_parameter_is_required") + " (" + parameterName + ")";
            LogResponse(stackTrace, code, message);
            return UnprocessableEntity(new
            {
                code,
                message
            });
        }

        protected ObjectResult ParameterFormatError(string parameterName, string code = "4001")
        {
            var stackTrace = new StackTrace();
            string message = LangTool.GetTranslation("common_parameter_format_error") + " (" + parameterName + ")";
            LogResponse(stackTrace, code, message);
            return UnprocessableEntity(new
            {
                code,
                message,
            });
        }

        protected ObjectResult UnprocessableEntity(string code, string message)
        {
            var stackTrace = new StackTrace();
            LogResponse(stackTrace, code, message);
            return UnprocessableEntity(new
            {
                code,
                message
            });
        }

        protected ObjectResult UnprocessableEntity(string code, string message, object result)
        {
            var stackTrace = new StackTrace();
            LogResponse(stackTrace, code, message, result);
            return UnprocessableEntity(new { code, message, result });
        }

        protected ObjectResult Ok(string code, string message)
        {
            var stackTrace = new StackTrace();
            LogResponse(stackTrace, code, message);
            return Ok(new { code, message });
        }

        protected ObjectResult Ok(string code, string message, object result)
        {
            var stackTrace = new StackTrace();
            LogResponse(stackTrace, code, message, result);
            return Ok(new { code, message, result });
        }

        protected ObjectResult ExceptionResponse(Exception ex)
        {
            var stackTrace = new StackTrace();
            // Exception 不該回傳回去，只做 Log 紀錄
            _logger.LogError(ex.ToString(), stackTrace);
            return ExceptionResponse("System error");
        }

        protected ObjectResult ExceptionResponse(string exceptionMessage)
        {
            var stackTrace = new StackTrace();
            LogResponse(stackTrace, ResponseCode.FAIL, exceptionMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                code = ResponseCode.FAIL,
                message = exceptionMessage
            });
        }

        private void LogResponse(StackTrace stackTrace, string code, string message, object? result = null)
        {
            _logger.LogInformation(
                GetTraceMethName(stackTrace),
                code,
                message,
                result
            );
        }

        private string GetTraceMethName(StackTrace stackTrace)
        {
            MethodBase method = stackTrace.GetFrame(1)!.GetMethod()!;
            return method.DeclaringType!.Name + "." + method.Name;
        }
    }
}
