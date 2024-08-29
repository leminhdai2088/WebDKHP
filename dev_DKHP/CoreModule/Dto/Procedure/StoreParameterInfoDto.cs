namespace dev_DKHP.CoreModule.Dto.Procedure
{
    public class StoreParameterInfoDto
    {
        public string PARAMETER_NAME { get; set; } = string.Empty;
        public string PARAMETER_MODE { get; set; } = string.Empty;
        public string DATA_TYPE { get; set; } = string.Empty;
        public int CHARACTER_MAXIMUM_LENGTH { get; set; }
    }
}
