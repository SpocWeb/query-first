using System.Data;

namespace QueryFirst
{
    internal interface IQfDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
