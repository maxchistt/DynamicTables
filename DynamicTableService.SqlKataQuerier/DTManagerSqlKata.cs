using DynamicTableService.Components;
using SqlKata;

namespace DynamicTableService.SqlKataQuerier
{
    public class DTManagerSqlKata : DTManagerNoQuerier
    {
        public readonly DTQuerier Querier;

        public DTManagerSqlKata(string connectionString) : base(connectionString)
        {
            Querier = new DTQuerier(connectionString);
        }

        public DTManagerSqlKata() : this(getConnectionStringFromConfig())
        {
        }
    }
}