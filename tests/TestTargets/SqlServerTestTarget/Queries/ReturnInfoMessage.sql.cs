namespace SqlServerTestTarget.Queries{
using System;
using System.Data;
using QueryFirst;
using Microsoft.Data.SqlClient;

public interface IReturnInfoMessageQfRepo{

int ExecuteNonQuery();
int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
}
public partial class ReturnInfoMessageQfRepo : IReturnInfoMessageQfRepo

{

void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
public string ExecutionMessages { get; protected set; }
// constructor with connection factory injection
protected QueryFirst.QueryFirstConnectionFactory _connectionFactory;
public  ReturnInfoMessageQfRepo(QueryFirst.QueryFirstConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}
private static IReturnInfoMessageQfRepo _inst;
private static IReturnInfoMessageQfRepo inst { get
{
if (_inst == null)
_inst = new ReturnInfoMessageQfRepo(QueryFirstConnectionFactory.Instance);
return _inst;
}
}

#region Sync

public static int ExecuteNonQueryStatic()
=> inst.ExecuteNonQuery();
public virtual int ExecuteNonQuery()
{
    using IDbConnection conn = _connectionFactory.CreateConnection();
    conn.Open();
    return ExecuteNonQuery(conn);
}

public static int ExecuteNonQueryStatic(IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteNonQuery(conn, tx);
public virtual int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null)
{

// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
using IDbCommand cmd = conn.CreateCommand();
if(tx != null)
    cmd.Transaction = tx;
cmd.CommandText = GetCommandText();
AddParameters( cmd);
var result = cmd.ExecuteNonQuery();

// Assign output parameters to instance properties. 
/*

*/
// only convert dbnull if nullable
return result;
}


#endregion

public static string GetCommandText(){
var queryText = $@"-- use queryfirst
/*designTime

endDesignTime*/

PRINT 'info message for cobber'
";
// QfExpandoParams

return queryText;
}
protected static void AddParameters(IDbCommand cmd)
{
}
}
}
