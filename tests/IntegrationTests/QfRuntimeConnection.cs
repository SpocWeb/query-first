using System.Data;
using System.Data.SqlClient;

namespace IntegrationTests
{
    class QfRuntimeConnection
    {
        public static IDbConnection GetConnection()
        {
            return new SqlConnection("Data Source=QueryFirstTestDB;Database=master;Trusted_Connection=True;");
        }
    }
}

