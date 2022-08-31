Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: TCP Provider, error: 0 - No such host is known.)
-----------------------------------------------------------
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal& connection)
   at System.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   at System.Data.SqlClient.SqlConnection.TryOpen(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.Open()
   at QueryFirst.AdoSchemaFetcher.GetFields(String connectionString, String provider, String Query) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedAll\SchemaFetching\AdoSchemaFetcher.cs:line 24
   at QueryFirst._7RunQueryAndGetResultSchema.Go(State& state) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedAll\StateAndTransitions\_7RunQueryAndGetResultSchema.cs:line 21
   at QueryFirst.CommandLine.CommandLineConductor.ProcessOneQuery(String sourcePath, QfConfigModel outerConfig) in C:\Users\sboddy\source\repos\query-first\QueryFirst.CommandLine\CommandLineConductor.cs:line 91
*/
