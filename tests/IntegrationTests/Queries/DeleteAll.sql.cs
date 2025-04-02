namespace QueryFirst.IntegrationTests.Queries{
using System;
using System.Data;
using QueryFirst;
using System.Data.SqlClient;

public interface IDeleteAllQfRepo{

int ExecuteNonQuery();
int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
}
public partial class DeleteAllQfRepo : IDeleteAllQfRepo

{

void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
public string ExecutionMessages { get; protected set; }
// constructor with connection factory injection
protected QueryFirst.QueryFirstConnectionFactory _connectionFactory;
public  DeleteAllQfRepo(QueryFirst.QueryFirstConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}
private static IDeleteAllQfRepo _inst;
private static IDeleteAllQfRepo inst { get
{
if (_inst == null)
_inst = new DeleteAllQfRepo(QueryFirstConnectionFactory.Instance);
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
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });
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
var queryText = $@"/* .sql query managed by QueryFirst add-in */
/*designTime

endDesignTime*/
DELETE from EveryDatatype;

";
// QfExpandoParams

return queryText;
}
protected static void AddParameters(IDbCommand cmd)
{
}
}
}
