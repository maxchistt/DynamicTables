using DynamicTableService.Components;
using System.Configuration;

namespace DynamicTableService
{
    public class DTManagerNoQuerier
    {
        public readonly DTBuilder Builder;
        public readonly DTScaner Scaner;

        public DTManagerNoQuerier(string connectionString)
        {
            Builder = new DTBuilder(connectionString);
            Scaner = new DTScaner(connectionString);
        }

        public DTManagerNoQuerier() : this(getConnectionStringFromConfig())
        {
        }

        protected static string getConnectionStringFromConfig()
        {
            var con = ConfigurationManager.ConnectionStrings["MSSQL"];
            if (con == null) throw new Exception("Null connection string \"MSSQL\" in ConfigurationManager.ConnectionStrings");
            return con.ConnectionString;
        }
    }
}