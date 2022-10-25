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

using static DynamicInIntQfRepo;


using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public interface IDynamicInIntQfRepo{

List<DynamicInIntQfDto> Execute(List<int?> listOfIds);
IEnumerable< DynamicInIntQfDto> Execute(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null);
System.Int32? ExecuteScalar(List<int?> listOfIds);
System.Int32? ExecuteScalar(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null);

DynamicInIntQfDto Create(IDataRecord record);
DynamicInIntQfDto GetOne(List<int?> listOfIds);
DynamicInIntQfDto GetOne(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null);
int ExecuteNonQuery(List<int?> listOfIds);
int ExecuteNonQuery(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null);
}
public partial class DynamicInIntQfRepo : IDynamicInIntQfRepo

{

void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
public string ExecutionMessages { get; protected set; }
// constructor with connection factory injection
protected QueryFirst.QueryFirstConnectionFactory _connectionFactory;
public  DynamicInIntQfRepo(QueryFirst.QueryFirstConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}
private static IDynamicInIntQfRepo _inst;
private static IDynamicInIntQfRepo inst { get
{
if (_inst == null)
_inst = new DynamicInIntQfRepo(QueryFirstConnectionFactory.Instance);
return _inst;
}
}

#region Sync

public static int ExecuteNonQueryStatic(List<int?> listOfIds)
=> inst.ExecuteNonQuery(listOfIds);
public virtual int ExecuteNonQuery(List<int?> listOfIds)
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
return ExecuteNonQuery(listOfIds, conn);
}
}

public static int ExecuteNonQueryStatic(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteNonQuery(listOfIds, conn, tx);
public virtual int ExecuteNonQuery(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
{

// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
using(IDbCommand cmd = conn.CreateCommand())
{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText(listOfIds);
AddParameters(listOfIds,  cmd);
var result = cmd.ExecuteNonQuery();

// Assign output parameters to instance properties. 
/*

*/
// only convert dbnull if nullable
return result;
}
}


#endregion

public string getCommandText(List<int?> listOfIds){
var queryText = $@"/* .sql query managed by QueryFirst add-in */
/*designTime - put parameter declarations and design time initialization here
declare @ListOfIds int;
endDesignTime*/
SELECT * FROM EveryDatatype E WHERE E.MyInt IN(@ListOfIds)";
// QfExpandoParams
queryText = queryText.Replace("@ListOfIds", string.Join(",",listOfIds));
return queryText;
}
#region Sync


public static List<DynamicInIntQfDto> ExecuteStatic(List<int?> listOfIds)
=> inst.Execute(listOfIds);

public virtual List<DynamicInIntQfDto> Execute(List<int?> listOfIds)
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = Execute(listOfIds, conn).ToList();
return returnVal;
}
}

public static IEnumerable<DynamicInIntQfDto> ExecuteStatic(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
=> inst.Execute(listOfIds, conn, tx);

public virtual IEnumerable<DynamicInIntQfDto> Execute(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
using(IDbCommand cmd = conn.CreateCommand())
{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText(listOfIds);
AddParameters(listOfIds,  cmd);
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


public static DynamicInIntQfDto GetOneStatic(List<int?> listOfIds)
=> inst.GetOne(listOfIds);
public virtual DynamicInIntQfDto GetOne(List<int?> listOfIds)
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = GetOne(listOfIds, conn);
return returnVal;
}
}
public static DynamicInIntQfDto GetOneStatic(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
=> inst.GetOne(listOfIds, conn, tx);
public virtual DynamicInIntQfDto GetOne(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
{
var all = Execute(listOfIds,  conn,tx);
DynamicInIntQfDto returnVal;
using (IEnumerator<DynamicInIntQfDto> iter = all.GetEnumerator())
{
iter.MoveNext();
returnVal = iter.Current;
}
return returnVal;
}
}

public static System.Int32? ExecuteScalarStatic(List<int?> listOfIds)
=> inst.ExecuteScalar(listOfIds);
public virtual System.Int32? ExecuteScalar(List<int?> listOfIds)
{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{
conn.Open();
var returnVal = ExecuteScalar(listOfIds, conn);
/*
;
*/
return returnVal;
}
}


public static System.Int32? ExecuteScalarStatic(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteScalar(listOfIds, conn, tx);
public virtual System.Int32? ExecuteScalar(List<int?> listOfIds,IDbConnection conn, IDbTransaction tx = null)
{
// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });// hello from MyGroovyProvider
using(IDbCommand cmd = conn.CreateCommand())
{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText(listOfIds);
AddParameters(listOfIds,  cmd);
var result = cmd.ExecuteScalar();

// only convert dbnull if nullable
// Assign output parameters to instance properties.
/*

*/
if( result == null || result == DBNull.Value)
return null;
else
return (System.Int32?)result;
}
}


#endregion

public virtual DynamicInIntQfDto Create(IDataRecord record)
{
var returnVal = CreatePoco(record);

    if(record[0] != null && record[0] != DBNull.Value)
    returnVal.Id =  (int)record[0];

    if(record[1] != null && record[1] != DBNull.Value)
    returnVal.MyBigint =  (long?)record[1];

    if(record[2] != null && record[2] != DBNull.Value)
    returnVal.MyBit =  (bool?)record[2];

    if(record[3] != null && record[3] != DBNull.Value)
    returnVal.MyDecimal =  (decimal?)record[3];

    if(record[4] != null && record[4] != DBNull.Value)
    returnVal.MyInt =  (int?)record[4];

    if(record[5] != null && record[5] != DBNull.Value)
    returnVal.MyMoney =  (decimal?)record[5];

    if(record[6] != null && record[6] != DBNull.Value)
    returnVal.MyNumeric =  (decimal?)record[6];

    if(record[7] != null && record[7] != DBNull.Value)
    returnVal.MySmallint =  (short?)record[7];

    if(record[8] != null && record[8] != DBNull.Value)
    returnVal.MyTinyint =  (byte?)record[8];

    if(record[9] != null && record[9] != DBNull.Value)
    returnVal.MyFloat =  (double?)record[9];

    if(record[10] != null && record[10] != DBNull.Value)
    returnVal.MyReal =  (float?)record[10];

    if(record[11] != null && record[11] != DBNull.Value)
    returnVal.MyDate =  (DateTime?)record[11];

    if(record[12] != null && record[12] != DBNull.Value)
    returnVal.MyDatetime2 =  (DateTime?)record[12];

    if(record[13] != null && record[13] != DBNull.Value)
    returnVal.MyDatetime =  (DateTime?)record[13];

    if(record[14] != null && record[14] != DBNull.Value)
    returnVal.MyChar =  (string)record[14];

    if(record[15] != null && record[15] != DBNull.Value)
    returnVal.MyVarchar =  (string)record[15];

    if(record[16] != null && record[16] != DBNull.Value)
    returnVal.MyText =  (string)record[16];

    if(record[17] != null && record[17] != DBNull.Value)
    returnVal.MyNchar =  (string)record[17];

    if(record[18] != null && record[18] != DBNull.Value)
    returnVal.MyNvarchar =  (string)record[18];

    if(record[19] != null && record[19] != DBNull.Value)
    returnVal.MyNtext =  (string)record[19];

    if(record[20] != null && record[20] != DBNull.Value)
    returnVal.MyBinary =  (System.Byte[])record[20];

    if(record[21] != null && record[21] != DBNull.Value)
    returnVal.MyVarbinary =  (System.Byte[])record[21];

    if(record[22] != null && record[22] != DBNull.Value)
    returnVal.MyImage =  (System.Byte[])record[22];

// provide a hook to override
returnVal.OnLoad();
return returnVal;
}
protected virtual DynamicInIntQfDto CreatePoco(System.Data.IDataRecord record)
{
    return new DynamicInIntQfDto();
}
protected void AddParameters(List<int?> listOfIds,IDbCommand cmd)
{
}
}
public partial class DynamicInIntQfDto  {
protected int _Id; // int(4) not null
public int Id{
get{return _Id;}
set{_Id = value;}
}
protected long? _MyBigint; // bigint(8) null
public long? MyBigint{
get{return _MyBigint;}
set{_MyBigint = value;}
}
protected bool? _MyBit; // bit(1) null
public bool? MyBit{
get{return _MyBit;}
set{_MyBit = value;}
}
protected decimal? _MyDecimal; // decimal(17) null
public decimal? MyDecimal{
get{return _MyDecimal;}
set{_MyDecimal = value;}
}
protected int? _MyInt; // int(4) null
public int? MyInt{
get{return _MyInt;}
set{_MyInt = value;}
}
protected decimal? _MyMoney; // money(8) null
public decimal? MyMoney{
get{return _MyMoney;}
set{_MyMoney = value;}
}
protected decimal? _MyNumeric; // decimal(17) null
public decimal? MyNumeric{
get{return _MyNumeric;}
set{_MyNumeric = value;}
}
protected short? _MySmallint; // smallint(2) null
public short? MySmallint{
get{return _MySmallint;}
set{_MySmallint = value;}
}
protected byte? _MyTinyint; // tinyint(1) null
public byte? MyTinyint{
get{return _MyTinyint;}
set{_MyTinyint = value;}
}
protected double? _MyFloat; // float(8) null
public double? MyFloat{
get{return _MyFloat;}
set{_MyFloat = value;}
}
protected float? _MyReal; // real(4) null
public float? MyReal{
get{return _MyReal;}
set{_MyReal = value;}
}
protected DateTime? _MyDate; // date(3) null
public DateTime? MyDate{
get{return _MyDate;}
set{_MyDate = value;}
}
protected DateTime? _MyDatetime2; // datetime2(8) null
public DateTime? MyDatetime2{
get{return _MyDatetime2;}
set{_MyDatetime2 = value;}
}
protected DateTime? _MyDatetime; // datetime(8) null
public DateTime? MyDatetime{
get{return _MyDatetime;}
set{_MyDatetime = value;}
}
protected string _MyChar; // char(255) null
public string MyChar{
get{return _MyChar;}
set{_MyChar = value;}
}
protected string _MyVarchar; // varchar(50) null
public string MyVarchar{
get{return _MyVarchar;}
set{_MyVarchar = value;}
}
protected string _MyText; // text(2147483647) null
public string MyText{
get{return _MyText;}
set{_MyText = value;}
}
protected string _MyNchar; // nchar(50) null
public string MyNchar{
get{return _MyNchar;}
set{_MyNchar = value;}
}
protected string _MyNvarchar; // nvarchar(50) null
public string MyNvarchar{
get{return _MyNvarchar;}
set{_MyNvarchar = value;}
}
protected string _MyNtext; // ntext(1073741823) null
public string MyNtext{
get{return _MyNtext;}
set{_MyNtext = value;}
}
protected System.Byte[] _MyBinary; // binary(50) null
public System.Byte[] MyBinary{
get{return _MyBinary;}
set{_MyBinary = value;}
}
protected System.Byte[] _MyVarbinary; // varbinary(50) null
public System.Byte[] MyVarbinary{
get{return _MyVarbinary;}
set{_MyVarbinary = value;}
}
protected System.Byte[] _MyImage; // image(2147483647) null
public System.Byte[] MyImage{
get{return _MyImage;}
set{_MyImage = value;}
}
protected internal virtual void OnLoad(){}
}
}
