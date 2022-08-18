using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QueryFirst;

/* What provider are you using? For SqlClient, you will need to add a project reference (.net framework) or 
the System.Data.SqlClient nuget package (.net core). */

namespace QueryFirst
{
    public interface IQfDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
namespace QueryFirst.IntegrationTests
{


    public class QfDbConnectionFactory : IQfDbConnectionFactory
    {
        //public IConfiguration _config { get; set; }
        //public QfDbConnectionFactory(IConfiguration config)
        //{
        //    _config = config;
        //}

        public IDbConnection CreateConnection()
        {
            //return new SqlConnection(_config.GetConnectionString("QueryFirstTestDB"));
            return new SqlConnection("Data Source=s-dev4,1434;Database=QueryFirstTestDB;Trusted_Connection=True;");
        }
    }

    public class MasterConnectionFactory : IQfDbConnectionFactory
    {
        //public IConfiguration _config { get; set; }
        //public QfDbConnectionFactory(IConfiguration config)
        //{
        //    _config = config;
        //}

        public IDbConnection CreateConnection()
        {
            //return new SqlConnection(_config.GetConnectionString("QueryFirstTestDB"));
            return new SqlConnection("Data Source=s-dev4,1434;Database=master;Trusted_Connection=True;");
        }
    }
}