using System.Data;

namespace dev_DKHP.CoreModule.Helper.Procedure
{
    public interface IStoredProcedureProvider
    {
        Task<List<TModel>> GetDataFromStoredProcedure<TModel>(string storedProcName, object parameters) where TModel : class;
        Task<DataSet> GetMultiDataFromStoredProcedure(string storedProcName, List<ReportParameter> parameters);
        Task<IDictionary<string, object>> GetResultValueFromStore(string storedProcName, object parameters);
    }
}
