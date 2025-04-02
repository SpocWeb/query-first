using System.Collections.Generic;

namespace QueryFirst
{
    public interface ISchemaFetcher
    {
        List<ResultFieldDetails> GetFields(string connectionString, string provider, string Query);
    }
}