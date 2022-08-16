using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst.CommandLine
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
