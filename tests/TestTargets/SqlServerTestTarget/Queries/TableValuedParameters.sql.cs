namespace SqlServerTestTarget.Queries{
using System;
using System.Data;
using System.Collections.Generic;
using QueryFirst;
using static TableValuedParametersQfRepo;

using FastMember; // Table valued params require the FastMember Nuget package

using Microsoft.Data.SqlClient;

public interface ITableValuedParametersQfRepo{

int ExecuteNonQuery(IEnumerable<TestTableType> myTableValuedParam);
int ExecuteNonQuery(IEnumerable<TestTableType> myTableValuedParam, IDbConnection conn, IDbTransaction tx = null);
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

public static int ExecuteNonQueryStatic(IEnumerable<TestTableType> myTableValuedParam)
=> inst.ExecuteNonQuery(myTableValuedParam);
public virtual int ExecuteNonQuery(IEnumerable<TestTableType> myTableValuedParam)
{
    using IDbConnection conn = _connectionFactory.CreateConnection();
    conn.Open();
    return ExecuteNonQuery(myTableValuedParam, conn);
}

public static int ExecuteNonQueryStatic(IEnumerable<TestTableType> myTableValuedParam, IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteNonQuery(myTableValuedParam, conn, tx);
public virtual int ExecuteNonQuery(IEnumerable<TestTableType> myTableValuedParam, IDbConnection conn, IDbTransaction tx = null)
{

// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
using IDbCommand cmd = conn.CreateCommand();
if(tx != null)
    cmd.Transaction = tx;
cmd.CommandText = GetCommandText(myTableValuedParam);
AddParameters(myTableValuedParam,  cmd);
var result = cmd.ExecuteNonQuery();

// Assign output parameters to instance properties. 
/*

*/
// only convert dbnull if nullable
return result;
}


#endregion

public static string GetCommandText(IEnumerable<TestTableType> myTableValuedParam){
var queryText = $@"/* .sql query managed by QueryFirst add-in */

/*designTime - put parameter declarations and design time initialization here
DECLARE @MyTableValuedParam TestTableType;
endDesignTime*/


      INSERT into EveryDatatype (MyVarchar, MyInt) 
      (SELECT TVP.MyTVPVarchar, TVP.MyTVPInt  FROM @MyTableValuedParam TVP)";
// QfExpandoParams

return queryText;
}public class TestTableType{
public System.String MyTVPVarchar{get; set;}
public System.Int32? MYTVPInt{get; set;}

}

protected static void AddParameters(IEnumerable<TestTableType> myTableValuedParam, IDbCommand cmd)
{

{
var myParam = (SqlParameter)cmd.CreateParameter();
myParam.Direction = ParameterDirection.Input;
myParam.ParameterName = "@MyTableValuedParam";
myParam.SqlDbType = SqlDbType.Structured;
myParam.TypeName = "TestTableType";
DataTable table = new DataTable();
using (var reader = ObjectReader.Create(myTableValuedParam, new string[]{"MyTVPVarchar","MYTVPInt"}))
{
    table.Load(reader);
}
myParam.Value = (object)table ?? DBNull.Value;

cmd.Parameters.Add(myParam);
}}
}
}
