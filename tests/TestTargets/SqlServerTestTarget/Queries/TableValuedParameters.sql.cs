namespace SqlServerTestTarget.Queries{
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using QueryFirst;
using System.Text.RegularExpressions;

using static TableValuedParametersQfRepo;


using System.Data.SqlClient;
using System.Threading.Tasks;

public interface ITableValuedParametersQfRepo{

int ExecuteNonQuery();
int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
}
public partial class TableValuedParametersQfRepo : ITableValuedParametersQfRepo

{

void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
public string ExecutionMessages { get; protected set; }
// constructor with connection factory injection
protected QueryFirst.QueryFirstConnectionFactory _connectionFactory;
public  TableValuedParametersQfRepo(QueryFirst.QueryFirstConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}
private static ITableValuedParametersQfRepo _inst;
private static ITableValuedParametersQfRepo inst { get
{
if (_inst == null)
_inst = new TableValuedParametersQfRepo(QueryFirstConnectionFactory.Instance);
return _inst;
}
}

#region Sync

public static int ExecuteNonQueryStatic()
=> inst.ExecuteNonQuery();
public virtual int ExecuteNonQuery()
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
return ExecuteNonQuery(conn);
}
}

public static int ExecuteNonQueryStatic(IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteNonQuery(conn, tx);
public virtual int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null)
{

// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
using(IDbCommand cmd = conn.CreateCommand())
{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText();
AddParameters( cmd);
var result = cmd.ExecuteNonQuery();

// Assign output parameters to instance properties. 
/*

*/
// only convert dbnull if nullable
return result;
}
}


#endregion

public string getCommandText(){
var queryText = $@"/* .sql query managed by QueryFirst add-in */
/*designTime - put parameter declarations and design time initialization here
endDesignTime*/

CREATE TYPE TestTableType 
   AS TABLE
      ( MyTVPVarchar VARCHAR(255)
      , MYTVPInt INT );";
// QfExpandoParams

return queryText;
}
protected void AddParameters(IDbCommand cmd)
{
}
}
}
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
La variable de table "@MyTableValuedParam" doit être déclarée.
-----------------------------------------------------------
   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at System.Data.SqlClient.SqlDataReader.get_MetaData()
   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)
   at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior, String method)
   at QueryFirst.AdoSchemaFetcher.GetQuerySchema(IDbConnection connection, IProvider prov, String strSQL) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 192
   at QueryFirst.AdoSchemaFetcher.GetFields(IDbConnection connection, IProvider provObj, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 60
   at QueryFirst.AdoSchemaFetcher.GetFields(String connectionString, String provider, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 25
   at QueryFirst._7RunQueryAndGetResultSchema.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_7RunQueryAndGetResultSchema.cs:line 21
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 125
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
La variable de table "@MyTableValuedParam" doit être déclarée.
-----------------------------------------------------------
   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at System.Data.SqlClient.SqlDataReader.get_MetaData()
   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)
   at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior, String method)
   at QueryFirst.AdoSchemaFetcher.GetQuerySchema(IDbConnection connection, IProvider prov, String strSQL) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 192
   at QueryFirst.AdoSchemaFetcher.GetFields(IDbConnection connection, IProvider provObj, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 60
   at QueryFirst.AdoSchemaFetcher.GetFields(String connectionString, String provider, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 25
   at QueryFirst._7RunQueryAndGetResultSchema.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_7RunQueryAndGetResultSchema.cs:line 21
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 125
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Colonne, paramètre ou variable #1 : type de données TestTableType introuvable.
La variable de table "@MyTableValuedParam" doit être déclarée.
Le paramètre ou la variable '@MyTableValuedParam' a un type de données non valide.
-----------------------------------------------------------
   at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at System.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at System.Data.SqlClient.SqlDataReader.get_MetaData()
   at System.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at System.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean async, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry)
   at System.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)
   at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior, String method)
   at QueryFirst.AdoSchemaFetcher.GetQuerySchema(IDbConnection connection, IProvider prov, String strSQL) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 192
   at QueryFirst.AdoSchemaFetcher.GetFields(IDbConnection connection, IProvider provObj, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 60
   at QueryFirst.AdoSchemaFetcher.GetFields(String connectionString, String provider, String Query) in C:\Users\sboddy\source\repos\query-first\Shared\SchemaFetching\AdoSchemaFetcher.cs:line 25
   at QueryFirst._7RunQueryAndGetResultSchema.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_7RunQueryAndGetResultSchema.cs:line 21
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 125
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 126
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 126
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.VSExtension.VsixConductor.ProcessOneQuery(Document queryDoc, Boolean headless) in C:\Users\sboddy\source\repos\query-first\QueryFirst.SharedVSExt\VsixConductor.cs:line 126
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.CommandLine.CommandLineConductor.ProcessOneQuery(String sourcePath, QfConfigModel outerConfig) in C:\Users\sboddy\source\repos\query-first\QueryFirst.CommandLine\CommandLineConductor.cs:line 92
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.CommandLine.CommandLineConductor.ProcessOneQuery(String sourcePath, QfConfigModel outerConfig) in C:\Users\sboddy\source\repos\query-first\QueryFirst.CommandLine\CommandLineConductor.cs:line 92
*/
Error running query.

/*The last attempt to run this query failed with the following error. This class is no longer synced with the query
You can compile the class by deleting this error information, but it will likely generate runtime errors.
-----------------------------------------------------------
Unable to find a type for TestTableType
-----------------------------------------------------------
   at QueryFirst.Providers.SqlClient.ParseDeclaredParameters(String queryText, String connectionString) in C:\Users\sboddy\source\repos\query-first\Shared\Providers\SqlClient.cs:line 39
   at QueryFirst._8ParseOrFindDeclaredParams.Go(State& state) in C:\Users\sboddy\source\repos\query-first\Shared\StateAndTransitions\_8ParseOrFindDeclaredParams.cs:line 26
   at QueryFirst.CommandLine.CommandLineConductor.ProcessOneQuery(String sourcePath, QfConfigModel outerConfig) in C:\Users\sboddy\source\repos\query-first\QueryFirst.CommandLine\CommandLineConductor.cs:line 92
*/
