using dev_DKHP.CoreModule.Dto.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace dev_DKHP.CoreModule.Helper.Authorization
{
    public class CustomException: Exception
    {
        public int STATUS_CODE { get; set; }
        public string ERROR_MESSAGE { get; set; } = string.Empty;
        public dynamic DATA { get; set; }
        public CustomException() 
        {
        }
        public CustomException(int statusCode, string errorMessage): base(errorMessage)
        {
            STATUS_CODE = statusCode;
            ERROR_MESSAGE = errorMessage;
        }

        public CustomException(int statusCode, string errorMessage, dynamic data) : base(errorMessage)
        {
            STATUS_CODE = statusCode;
            ERROR_MESSAGE = errorMessage;
            DATA = data;
        }
    }
}
