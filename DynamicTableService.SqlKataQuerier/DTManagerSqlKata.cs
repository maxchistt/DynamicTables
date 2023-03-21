using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;

namespace DynamicTableService.SqlKataQuerier
{
    public class DTManagerSqlKata : DTManagerNoQuerier
    {
        public readonly QueryFactory Querier;

        public DTManagerSqlKata(string connectionString) : base(connectionString)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            Compiler compiler = new SqlServerCompiler();

            Querier = new QueryFactory(connection, compiler);
        }

        public DTManagerSqlKata() : this(getConnectionStringFromConfig())
        {
        }
    }
}