namespace dev_DKHP.CoreModule.Helper.Procedure
{
    public class ArgumentExceptionEx: ArgumentException
    {
        public int ErrorCode { get; }
        public string PropertyName { get; } = string.Empty;
        public ArgumentExceptionEx(string paramName, int errorCode, string propertyName)
            : base(paramName)
        {
            ErrorCode = errorCode;
            PropertyName = propertyName;
        }
    }

    public class ReportParameter
    {
        public string Name { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
    }
}
