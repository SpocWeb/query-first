namespace SqlServerTestTarget.Queries{
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using static GetOneRowAsyncQfRepo;


using System.Data.SqlClient;
using System.Threading.Tasks;

public interface IGetOneRowAsyncQfRepo{

List<GetOneRowAsyncQfDto> Execute();
IEnumerable< GetOneRowAsyncQfDto> Execute(IDbConnection conn, IDbTransaction tx = null);
System.String ExecuteScalar();
System.String ExecuteScalar(IDbConnection conn, IDbTransaction tx = null);

GetOneRowAsyncQfDto Create(IDataRecord record);
GetOneRowAsyncQfDto GetOne();
GetOneRowAsyncQfDto GetOne(IDbConnection conn, IDbTransaction tx = null);
int ExecuteNonQuery();
int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
#region Async
Task<List<GetOneRowAsyncQfDto>> ExecuteAsync();
Task<IEnumerable< GetOneRowAsyncQfDto>> ExecuteAsync(IDbConnection conn, IDbTransaction tx = null);
Task<System.String> ExecuteScalarAsync();
Task<System.String> ExecuteScalarAsync(IDbConnection conn, IDbTransaction tx = null);

Task<GetOneRowAsyncQfDto> GetOneAsync();
Task<GetOneRowAsyncQfDto> GetOneAsync(IDbConnection conn, IDbTransaction tx = null);
Task<int> ExecuteNonQueryAsync();
Task<int> ExecuteNonQueryAsync(IDbConnection conn, IDbTransaction tx = null);
# endregion
}
public partial class GetOneRowAsyncQfRepo : IGetOneRowAsyncQfRepo
{

void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
public string ExecutionMessages { get; protected set; }
// constructor with connection factory injection
protected QueryFirst.IQfDbConnectionFactory _connectionFactory;
public  GetOneRowAsyncQfRepo(QueryFirst.IQfDbConnectionFactory connectionFactory){
    _connectionFactory = connectionFactory;
}

#region Sync
public virtual int ExecuteNonQuery()
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
return ExecuteNonQuery(conn);
}
}
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


#region ASync

public virtual async Task<int> ExecuteNonQueryAsync()
{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = await ExecuteNonQueryAsync( conn);
/*
;
*/
return returnVal;
}
}
public virtual async Task<int> ExecuteNonQueryAsync(IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
using(DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters( cmd);
var result = await cmd.ExecuteNonQueryAsync();

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
SELECT E.MyNvarchar FROM EveryDatatype E
WHERE MyNvarchar like 'async result%'
";
// QfExpandoParams

return queryText;
}
#region Sync

public virtual List<GetOneRowAsyncQfDto> Execute()
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = Execute(conn).ToList();
return returnVal;
}
}
public virtual IEnumerable<GetOneRowAsyncQfDto> Execute(IDbConnection conn, IDbTransaction tx = null){

// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
using(IDbCommand cmd = conn.CreateCommand())
{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText();
AddParameters( cmd);
using (var reader = cmd.ExecuteReader())
{
while (reader.Read())
{
yield return Create(reader);
}
}

// Assign output parameters to instance properties. These will be available after this method returns.
// todo : make output parameters work in a threadsafe way. An output object with execution messages and output parameters?
/*

*/
}
}

public virtual GetOneRowAsyncQfDto GetOne()
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = GetOne(conn);
return returnVal;
}
}public virtual GetOneRowAsyncQfDto GetOne(IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
{
var all = Execute( conn,tx);
GetOneRowAsyncQfDto returnVal;
using (IEnumerator<GetOneRowAsyncQfDto> iter = all.GetEnumerator())
{
iter.MoveNext();
returnVal = iter.Current;
}
return returnVal;
}
}
public virtual System.String ExecuteScalar()
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = ExecuteScalar(conn);
/*
;
*/
return returnVal;
}
}

public virtual System.String ExecuteScalar(IDbConnection conn, IDbTransaction tx = null)
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
var result = cmd.ExecuteScalar();

// only convert dbnull if nullable
// Assign output parameters to instance properties.
/*

*/
if( result == null || result == DBNull.Value)
return null;
else
return (System.String)result;
}
}


#endregion

#region ASync

public virtual async Task<List<GetOneRowAsyncQfDto>> ExecuteAsync()
{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{
await conn.OpenAsync();
var returnVal = await ExecuteAsync(conn);
/*

*/
return returnVal.ToList();
}
}
public virtual async Task<IEnumerable<GetOneRowAsyncQfDto>> ExecuteAsync( IDbConnection conn, IDbTransaction tx = null){
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
using (DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters( cmd);
SqlDataReader reader = (SqlDataReader)await cmd.ExecuteReaderAsync();
                

// Assign output parameters to instance properties. These will be available after this method returns.
/*

*/

return ReadItems(reader).ToArray();
}
}
IEnumerable<GetOneRowAsyncQfDto> ReadItems(SqlDataReader reader)
{
    while (reader.Read())
    {
        yield return Create(reader);
    }
}


public virtual async Task<GetOneRowAsyncQfDto> GetOneAsync()
{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{
    await conn.OpenAsync();
    return await GetOneAsync( conn);
}
}
public virtual async Task<GetOneRowAsyncQfDto> GetOneAsync(IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
var all = await ExecuteAsync( conn,tx);
GetOneRowAsyncQfDto returnVal;
using (IEnumerator<GetOneRowAsyncQfDto> iter = all.GetEnumerator())
{
iter.MoveNext();
returnVal = iter.Current;
}
return returnVal;
}
public virtual async Task<System.String> ExecuteScalarAsync()
{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = await ExecuteScalarAsync( conn);
/*
;
*/
return returnVal;
}
}

public virtual async Task<System.String> ExecuteScalarAsync( IDbConnection conn, IDbTransaction tx = null)
{
using(DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters( cmd);
var result = await cmd.ExecuteScalarAsync();

// only convert dbnull if nullable
// Assign output parameters to instance properties. 
/*

*/
if( result == null || result == DBNull.Value)
return null;
else
return (System.String)result;
}
}


#endregion

public virtual GetOneRowAsyncQfDto Create(IDataRecord record)
{
var returnVal = CreatePoco(record);

    if(record[0] != null && record[0] != DBNull.Value)
    returnVal.MyNvarchar =  (string)record[0];

// provide a hook to override
returnVal.OnLoad();
return returnVal;
}
protected virtual GetOneRowAsyncQfDto CreatePoco(System.Data.IDataRecord record)
{
    return new GetOneRowAsyncQfDto();
}
protected void AddParameters(IDbCommand cmd)
{
}
}
public partial class GetOneRowAsyncQfDto  {
protected string _MyNvarchar; // nvarchar(50) null
public string MyNvarchar{
get{return _MyNvarchar;}
set{_MyNvarchar = value;}
}
protected internal virtual void OnLoad(){}
}
}
