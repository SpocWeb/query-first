using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Net5CmdLineTestTarget
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var result = new GetCustomers().Execute();
            result.ForEach(l => Console.WriteLine($"{l.ContactName} {l.City}"));

            //var infoMsgResult = new ReturnInfoMessage().ExecuteNonQuery();

            // For info messages, the runtime connection needs to be consistent with the design time.
            // In this project QfRuntimeConnection is System.Data.SqlClient. The following query forces the provider to 
            // Microsoft.Data.SqlClient, so the runtime connection has to follow.
            using (var conn = new Microsoft.Data.SqlClient.SqlConnection("Server = localhost\\SQLEXPRESS; Database = NORTHWND; Trusted_Connection = True; "))
            {
                conn.Open();
                var MSInfoMsgResult = new ReturnInfoMessage_MicrosoftData().ExecuteNonQuery(conn);
            }


            // Test Dynamic OrderBy
            var query = new TestDynamicOrderBy();
            var sorted = query.Execute(new[] { (TestDynamicOrderBy.Cols.ContactName, false) });
            sorted.ForEach(l => Console.WriteLine($"{l.ContactName} {l.City}"));

            var asyncResult = new GetCustomersAsync().ExecuteAsync().Result;

        }
    }

}
