﻿using DynamicTableService.Components;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DynamicTableService
{
    public class DynamicTableManager
    {
        public readonly DTBuilder Builder;
        public readonly DTScaner Scaner;
        public readonly DTQuerier Querier;

        public DynamicTableManager(string connectionString)
        {
            Builder = new DTBuilder(connectionString);
            Scaner = new DTScaner(connectionString);
            Querier = new DTQuerier(connectionString);
        }

        public DynamicTableManager() : this(getConnectionStringFromConfig()){ }

        private static string getConnectionStringFromConfig()
        {
            var con = ConfigurationManager.ConnectionStrings["MSSQL"];
            if (con == null) throw new Exception("Null connection string \"MSSQL\" in ConfigurationManager.ConnectionStrings");
            return con.ConnectionString;
        }
    }
}