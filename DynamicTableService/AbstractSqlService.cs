using System.Data.SqlClient;

namespace DynamicTableService.Components
{
    public abstract class AbstractSqlService
    {
        protected string connectionString;

        public AbstractSqlService(string connectionString)
        {
            this.connectionString = connectionString;
            this.testConnection();
        }

        protected void testConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException e)
                {
                    throw new Exception("Db connection test fail", e);
                }
            }
        }

        protected int executeSQL(string sqlExpression)
        {
            int res = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                res = command.ExecuteNonQuery();
            }
            return res;
        }

        protected List<Dictionary<string, object>> executeSQLReadDictionaries(string sqlExpression)
        {
            List<Dictionary<string, object>> rowsListRes = new();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            var row = new Dictionary<string, object>();

                            for (int fieldNum = 0; fieldNum < reader.FieldCount; fieldNum++)
                            {
                                if (!reader.IsDBNull(fieldNum))
                                {
                                    string fieldName = reader.GetName(fieldNum);
                                    object fval = reader.GetValue(fieldNum);
                                    row.Add(fieldName, fval);
                                }
                            }

                            rowsListRes.Add(row);
                        }
                    }
                }
            }

            return rowsListRes;
        }

        protected List<object[]> executeSQLReadArrays(string sqlExpression)
        {
            List<object[]> rowsListRes = new();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            var row = new object[reader.FieldCount];

                            for (int fieldNum = 0; fieldNum < reader.FieldCount; fieldNum++)
                            {
                                if (!reader.IsDBNull(fieldNum))
                                {
                                    object fval = reader.GetValue(fieldNum);
                                    row[fieldNum] = fval;
                                }
                            }

                            rowsListRes.Add(row);
                        }
                    }
                }
            }

            return rowsListRes;
        }
    }
}