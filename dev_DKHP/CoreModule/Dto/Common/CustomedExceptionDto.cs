namespace dev_DKHP.CoreModule.Dto.Common
{
    public class CustomedExceptionDto
    {
        public int STATUS_CODE { get; set; }
        public string ERROR_MESSAGE { get; set; } = string.Empty;
        public dynamic DATA { get; set; }
    }

    public class CommonReturnDto
    {
        public int STATUS_CODE { get; set; }
        public string ERROR_MESSAGE { get; set; } = string.Empty;
        public dynamic DATA { get; set; }
    }
}
