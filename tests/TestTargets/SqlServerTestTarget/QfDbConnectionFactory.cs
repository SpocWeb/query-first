using System.Data;
using Microsoft.Data.SqlClient;

namespace QueryFirst.IntegrationTests
{
    public class TestDB : QueryFirstConnectionFactory
    {
        public TestDB()
        {
            QueryFirstConnectionFactory.Instance = this;
        }
        public override IDbConnection CreateConnection()
        {
            //return new SqlConnection( _config.GetConnectionString("QueryFirstTestDB"));
            return new SqlConnection("Data Source=s-dev4,1434;Database=QueryFirstTestDB;Trusted_Connection=True;Encrypt=false;");
        }
    }
}