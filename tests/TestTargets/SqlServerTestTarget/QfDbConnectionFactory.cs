using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace QueryFirst.IntegrationTests
{


    public class TestDB : QueryFirstConnectionFactory
    {
        public override IDbConnection CreateConnection()
        {
            //return new SqlConnection( _config.GetConnectionString("QueryFirstTestDB"));
            return new SqlConnection("Data Source=s-dev4,1434;Database=QueryFirstTestDB;Trusted_Connection=True;");
        }
    }
}