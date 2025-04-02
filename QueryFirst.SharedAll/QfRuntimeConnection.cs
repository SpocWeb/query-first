using System.Data.SqlClient;

namespace QueryFirst
{
    public static class QfRuntimeConnection
    {
        public static string CurrentConnectionString { get; set; }
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(CurrentConnectionString);
        }
        public static string GetConnectionString()
        {
            return CurrentConnectionString;
        }
    }
}
