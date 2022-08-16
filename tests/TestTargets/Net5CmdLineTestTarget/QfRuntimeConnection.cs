using System.Data;
using System.Data.SqlClient;

namespace Net5CmdLineTestTarget
{
    class QfRuntimeConnection
    {
        public static IDbConnection GetConnection()
        {
            return new SqlConnection("Server=localhost\\SQLEXPRESS;Database=NORTHWND;Trusted_Connection=True;");
        }
    }
}

