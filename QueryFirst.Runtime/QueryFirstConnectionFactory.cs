using System;
using System.Data;

namespace QueryFirst
{
    public abstract class QueryFirstConnectionFactory
    {
        public QueryFirstConnectionFactory()
        {
            Instance = this;
        }
        public abstract IDbConnection CreateConnection();
        public static QueryFirstConnectionFactory Instance { get; set; }
    }
}
