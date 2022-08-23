using System;
using System.Data;

namespace QueryFirst
{
    public abstract class QueryFirstConnectionFactory
    {
        public abstract IDbConnection CreateConnection();
        public static QueryFirstConnectionFactory Instance { get; set; }
    }
}
