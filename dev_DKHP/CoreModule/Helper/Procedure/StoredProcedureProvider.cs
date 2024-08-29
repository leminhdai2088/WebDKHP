using dev_DKHP.CoreModule.Dto.Procedure;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using System.Reflection;
using dev_DKHP.CoreModule.Const;
using System;
using dev_DKHP.CoreModule.Helper.Authorization;

namespace dev_DKHP.CoreModule.Helper.Procedure
{
    public class StoredProcedureProvider : IStoredProcedureProvider
    {
        public string? ConnectionString { get; set; }
        private readonly IConfiguration _configuration;
        private readonly int commandTimeout = 30;
        public StoredProcedureProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetValue<string>("ConnectionStrings:Default");
        }
        private async Task<List<StoreParameterInfoDto>> GetParameterInfos(string storeProcName)
        {
            string queryString = "select PARAMETER_NAME, PARAMETER_MODE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from information_schema.parameters where specific_name = @storeProcName";
            using (var conn = new SqlConnection(ConnectionString))
            {
                var result = await conn.QueryAsync<StoreParameterInfoDto>(queryString, new { storeProcName });
                return result.ToList();
            }
        }

        private string GetParameterName(PropertyInfo property)
        {
            var paramName = "";

            var storeParameterAttribute = (StoreParamAttribute?)property.GetCustomAttributes(typeof(StoreParamAttribute), false).FirstOrDefault();

            if (storeParameterAttribute == null)
            {
                paramName = property.Name;
            }
            else
            {
                paramName = storeParameterAttribute.Name;
            }
            return "@" + paramName;
        }

        private StoreParameterInfoDto GetParameterInfo(List<StoreParameterInfoDto> parameterInfos, string paramName)
        {
            var result = parameterInfos
                .Where(x => x.PARAMETER_NAME.Replace("@", "").ToLower().Equals(paramName.Replace("@", "").ToLower())
                || x.PARAMETER_NAME.Replace("@", "").ToLower().Equals("p_" + paramName.Replace("@", "").ToLower())
                || x.PARAMETER_NAME.Replace("@", "").ToLower().Equals("l_" + paramName.Replace("@", "").ToLower()))
                .SingleOrDefault();
            return result;
        }

        private ParameterDirection GetParameterDirection(StoreParameterInfoDto parameterInfo)
        {
            switch (parameterInfo.PARAMETER_MODE)
            {
                case ParameterSqlDirection.Input:
                    return ParameterDirection.Input;
                case ParameterSqlDirection.InputOutput:
                    return ParameterDirection.InputOutput;
                case ParameterSqlDirection.Output:
                    return ParameterDirection.Output;
            }
            return ParameterDirection.Input;
        }

        private object GetParameterValue(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value.GetType() == typeof(DateTime?))
            {
                return ((DateTime?)value).Value.ToString(CommonConst.DateTimeFormat);
            }
            if (value.GetType() == typeof(DateTime))
            {
                return ((DateTime)value).ToString(CommonConst.DateTimeFormat);
            }
            return value;
        }

        private object GetParameterValue(PropertyInfo property, object obj)
        {
            var value = property.GetValue(obj);
            return GetParameterValue(value);
        }

        public async Task<List<TModel>> GetDataFromStoredProcedure<TModel>(string storedProcName, object parameters) where TModel : class
        {
            // Lấy thông tin param từ stored
            var parameterInfos = await GetParameterInfos(storedProcName);

            // Khai báo tập param sẽ truyền xuống stored
            var dapperParams = new DynamicParameters();

            // Khai báo param truyền vào là out output của store
            var outputPropertyTable = new Dictionary<string, PropertyInfo>();

            if (parameters != null)
            {
                // lấy tất cả thông tin của param truyền xuống khác null
                var properties = parameters.GetType().GetProperties().Where(x => x != null);

                // Khai báo thông tin list param truyền xuống có nằm trong param của stored
                List<StoreParameterInfoDto> procedureInfoInProperties = new List<StoreParameterInfoDto>();
                foreach (var property in properties)
                {
                    // lấy tên thuộc tính
                    var paramName = GetParameterName(property);

                    // Lấy thông tin param truyền xuống có nằm trong param của stored
                    var parameterInfo = GetParameterInfo(parameterInfos, paramName);

                    if (parameterInfo != null)
                        procedureInfoInProperties.Add(parameterInfo);
                    else continue;

                    // xác định loại param đó có phải là output của store hay không, nếu có thì add vào
                    var direction = GetParameterDirection(parameterInfo);
                    if (direction == ParameterDirection.InputOutput || direction == ParameterDirection.Output)
                    {
                        outputPropertyTable.Add(parameterInfo.PARAMETER_NAME, property);
                    }

                    // Lấy giá trị của param truyền xuống
                    var parameterValue = GetParameterValue(property, parameters);

                    // add param hợp lệ vào tập param sẽ truyền xuống stored
                    dapperParams.Add(parameterInfo.PARAMETER_NAME, parameterValue, null, direction);
                }

                var paramNames = dapperParams.ParameterNames.ToList();

                // Kiểm tra nếu không có tham số truyền xuống store thì add tham số đó vào và gán bằng null
                foreach (var parameterInfo in parameterInfos)
                {
                    if (!paramNames.Any(x => "@" + x == parameterInfo.PARAMETER_NAME))
                    {
                        dapperParams.Add(parameterInfo.PARAMETER_NAME, null, null, GetParameterDirection(parameterInfo));
                    }
                }

            }
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var result = (List<TModel>)conn.Query<TModel>(storedProcName, dapperParams, null, true,
                        commandTimeout, CommandType.StoredProcedure);
                    //foreach (var pair in outputPropertyTable)
                    //{
                    //    pair.Value.SetValue(parameters, dapperParams.Get<object>(pair.Key));
                    //}
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IDictionary<string, object>> GetResultValueFromStore(string storedProcName, object parameters)
        {
            try
            {
                //_ = await ValidateParameter(storedProcName, parameters);

                var model = (await GetDataFromStoredProcedure<dynamic>(storedProcName, parameters)).FirstOrDefault() as IDictionary<string, object>;
                return model;
            }
            catch (ArgumentExceptionEx e)
            {
                if (e.ErrorCode == 101)
                {
                    Dictionary<string, object> error = new Dictionary<string, object>();

                    error.Add("Result", "-2");
                    error.Add("ErrorDesc", e.Message);
                    error.Add("PropertyName", e.PropertyName);

                    return error;
                }
                else
                {
                    Dictionary<string, object> error = new Dictionary<string, object>();

                    error.Add("Result", "-1");
                    error.Add("ErrorDesc", e.Message);

                    return error;
                }

            }
        }

        public async Task<DataSet> GetMultiDataFromStoredProcedure(string storedProcName, List<ReportParameter> parameters)
        {

            var parameterInfos = await GetParameterInfos(storedProcName);
            var dapperParams = new DynamicParameters();

            if (parameters != null)
            {

                List<StoreParameterInfoDto> procedureInfoInProperties = new List<StoreParameterInfoDto>();
                foreach (var property in parameters)
                {

                    var parameterInfo = GetParameterInfo(parameterInfos, property.Name);

                    if (parameterInfo == null)
                    {
                        continue;
                    }

                    procedureInfoInProperties.Add(parameterInfo);

                    dapperParams.Add(parameterInfo.PARAMETER_NAME, GetParameterValue(property.Value));
                }
            }


            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    var da = new SqlDataAdapter(storedProcName, conn);
                    var ds = new DataSet();

                    da.SelectCommand.CommandType = CommandType.StoredProcedure;

                    da.SelectCommand.CommandTimeout = commandTimeout;

                    foreach (var item in dapperParams.ParameterNames)
                    {
                        da.SelectCommand.Parameters.Add(new SqlParameter(item, dapperParams.Get<object>(item)));
                    }
                    //da.SelectCommand.CommandTimeout
                    da.Fill(ds);
                    return ds;
                }
            }
            catch (Exception e)
            {
                throw new CustomException(-1, e.Message);
            }
        }
    }
}
