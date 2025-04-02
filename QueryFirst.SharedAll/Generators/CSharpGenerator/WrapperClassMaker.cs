using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QueryFirst.Generators
{
    public partial class WrapperClassMaker
    {
        static readonly string n = Environment.NewLine;
        public virtual string Usings(State state)
        {
            return $@"using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using QueryFirst;
using System.Text.RegularExpressions;
{n}using static {state._4RepoClassName};
{(state._8HasTableValuedParams ? $@"{n}using FastMember; // Table valued params require the FastMember Nuget package{n}" : n)}
{state._6ProviderSpecificUsings}
";
        }

        public virtual string StartNamespace(State state)
        {
            if (!string.IsNullOrEmpty(state._4Namespace))
                return "namespace " + state._4Namespace + "{" + Environment.NewLine;
            else
                return "";
        }
        public virtual string StartClass(State state)
        {
            return
$@"public partial class {state._4RepoClassName} : I{state._4RepoClassName}{Environment.NewLine}
{{

void AppendExececutionMessage(string msg) {{ ExecutionMessages += msg + Environment.NewLine; }}
public string ExecutionMessages {{ get; protected set; }}
// constructor with connection factory injection
protected QueryFirst.QueryFirstConnectionFactory _connectionFactory;
public  {state._4RepoClassName}(QueryFirst.QueryFirstConnectionFactory connectionFactory)
{{
    _connectionFactory = connectionFactory;
}}
private static I{state._4RepoClassName} _inst;
private static I{state._4RepoClassName} inst {{ get
{{
if (_inst == null)
_inst = new {state._4RepoClassName}(QueryFirstConnectionFactory.Instance);
return _inst;
}}
}}
";

        }
        public virtual string MakeExecuteWithoutConn(State state)
        {
            StringBuilder code = new StringBuilder();
            char[] spaceComma = new char[] { ',', ' ' };
            // Execute method, without connection
            code.AppendLine($@"
public static List<{state._4ResultInterfaceName}> ExecuteStatic({state._8MethodSignature.Trim(spaceComma)})
=> inst.Execute({state._8CallingArgs.Trim(spaceComma)});

public virtual List<{state._4ResultInterfaceName}> Execute({state._8MethodSignature.Trim(spaceComma)}){n}{{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{{
conn.Open();
var returnVal = Execute({state._8CallingArgs}conn).ToList();
return returnVal;
}}
}}");
            return code.ToString();
        }
        public virtual string MakeExecuteWithConn(State state)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine($@"
public static IEnumerable<{state._4ResultInterfaceName}> ExecuteStatic({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> inst.Execute({state._8CallingArgs}conn, tx);

public virtual IEnumerable<{state._4ResultInterfaceName}> Execute({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{
{state._8HookupExecutionMessagesMethodText}
using(IDbCommand cmd = conn.CreateCommand())
{{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText({state._8CallingArgs.Trim(spaceComma)});
AddParameters({state._8CallingArgs} cmd);
using (var reader = cmd.ExecuteReader())
{{
while (reader.Read())
{{
yield return Create(reader);
}}
}}

// Assign output parameters to instance properties. These will be available after this method returns.
// todo : make output parameters work in a threadsafe way. An output object with execution messages and output parameters?
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;{n}"))}
*/
}}
}}
");

            return code.ToString();
        }
        public virtual string MakeGetOneWithoutConn(State state)
        {
            // don't make GetOne if there are output params.
            if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
            {
                char[] spaceComma = new char[] { ',', ' ' };
                string code = "";
                code += $@"
public static {state._4ResultInterfaceName} GetOneStatic({state._8MethodSignature.Trim(spaceComma)})
=> inst.GetOne({state._8CallingArgs.Trim(spaceComma)});
public virtual {state._4ResultInterfaceName} GetOne({state._8MethodSignature.Trim(spaceComma)})
{{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{{
conn.Open();
var returnVal = GetOne({state._8CallingArgs}conn);
return returnVal;
}}
}}";
                return code;
            }
            else return "";

        }
        public virtual string MakeGetOneWithConn(State state)
        {
            string code = "";
            // don't make GetOne if there are output params.
            if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
            {

                code += $@"
public static {state._4ResultInterfaceName} GetOneStatic({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> inst.GetOne({state._8CallingArgs}conn, tx);
public virtual {state._4ResultInterfaceName} GetOne({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{
{state._8HookupExecutionMessagesMethodText}
{{
var all = Execute({state._8CallingArgs} conn,tx);
{state._4ResultInterfaceName} returnVal;
using (IEnumerator<{state._4ResultInterfaceName}> iter = all.GetEnumerator())
{{
iter.MoveNext();
returnVal = iter.Current;
}}
return returnVal;
}}
}}
";
            }
            return code;
        }
        public virtual string MakeExecuteScalarWithoutConn(State state)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine($@"
public static {state._7ExecuteScalarReturnType} ExecuteScalarStatic({state._8MethodSignature.Trim(spaceComma)})
=> inst.ExecuteScalar({state._8CallingArgs.Trim(spaceComma)});
public virtual {state._7ExecuteScalarReturnType} ExecuteScalar({state._8MethodSignature.Trim(spaceComma)})
{{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{{
conn.Open();
var returnVal = ExecuteScalar({state._8CallingArgs}conn);
/*
{string.Join(";" + n, state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNameCamel} = {qp.CSNamePascal}"))};
*/
return returnVal;
}}
}}
");

            return code.ToString();
        }

        public virtual string MakeExecuteScalarWithConn(State state)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine($@"
public static {state._7ExecuteScalarReturnType} ExecuteScalarStatic({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteScalar({state._8CallingArgs}conn, tx);
public virtual {state._7ExecuteScalarReturnType} ExecuteScalar({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{
{state._8HookupExecutionMessagesMethodText}
using(IDbCommand cmd = conn.CreateCommand())
{{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText({state._8CallingArgs.Trim(spaceComma)});
AddParameters({state._8CallingArgs} cmd);
var result = cmd.ExecuteScalar();

// only convert dbnull if nullable
// Assign output parameters to instance properties.
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;{n}"))}
*/
if( result == null || result == DBNull.Value)
return null;
else
return ({state._7ExecuteScalarReturnType})result;
}}
}}
"
            );// close ExecuteScalar()
            return code.ToString();
        }

        public virtual string MakeExecuteNonQueryWithoutConn(State state)
        {
            string code = "";
            code += $@"
public static int ExecuteNonQueryStatic({state._8MethodSignature.Trim(spaceComma)})
=> inst.ExecuteNonQuery({state._8CallingArgs.Trim(spaceComma)});
public virtual int ExecuteNonQuery({state._8MethodSignature.Trim(spaceComma)})
{{
using (IDbConnection conn = _connectionFactory.CreateConnection())
{{
conn.Open();
return ExecuteNonQuery({state._8CallingArgs}conn);
}}
}}
";
            return code;
        }
        public virtual string MakeExecuteNonQueryWithConn(State state)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteNonQuery() with connection
            code.AppendLine($@"
public static int ExecuteNonQueryStatic({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> inst.ExecuteNonQuery({state._8CallingArgs}conn, tx);
public virtual int ExecuteNonQuery({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{

{state._8HookupExecutionMessagesMethodText}
using(IDbCommand cmd = conn.CreateCommand())
{{
if(tx != null)
cmd.Transaction = tx;
cmd.CommandText = getCommandText({state._8CallingArgs.Trim(spaceComma)});
AddParameters({state._8CallingArgs} cmd);
var result = cmd.ExecuteNonQuery();

// Assign output parameters to instance properties. 
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;{n}"))}
*/
// only convert dbnull if nullable
return result;
}}
}}
"
            );// close ExecuteNonQuery()
            return code.ToString();
        }



        public virtual string MakeCreateMethod(State state)
        {
            return
$@"public virtual {state._4ResultInterfaceName} Create(IDataRecord record)
{{
var returnVal = CreatePoco(record);
{string.Join("", state._7ResultFields.Select((f, index) => $@"
    if(record[{index}] != null && record[{index}] != DBNull.Value)
    returnVal.{f.CSColumnName} =  ({f.TypeCsShort})record[{index}];
"))}
// provide a hook to override
returnVal.OnLoad();
return returnVal;
}}
protected virtual {state._4ResultClassName} CreatePoco(System.Data.IDataRecord record)
{{
    return new {state._4ResultClassName}();
}}";
        }
        public virtual string MakeGetCommandTextMethod(State state)
        {
            // each QfExpandoParam will generate a queryText.Replace(@myParam, string.Join(listOfValues))
            var fillExpandoParams = new List<string>();
            foreach (var qp in state._8QueryParams.Where(qp => qp.IsQfExpandoParam))
            {
                if (qp.CSType == "string")
                {
                    // for character types, we need to put the value in single quotes and double single quotes in the data.
                    fillExpandoParams.Add(
                        $@"queryText = queryText.Replace(""{qp.DbName}"", string.Join("","",{qp.CSNameCamel}.Select(val=>""'""+val.Replace(""'"",""''"")+""'"")));"
                    );
                }
                else
                {
                    fillExpandoParams.Add(
                        $@"queryText = queryText.Replace(""{qp.DbName}"", string.Join("","",{qp.CSNameCamel}));"
                    );
                }
            }
            if (state._5HasDynamicOrderBy)
            {

                return $@"public string getCommandText({state._8MethodSignature.Trim(spaceComma)}){{
var queryText = @""{state._6FinalQueryTextForCode}"";

// Dynamic order by
if(orderBy != null && orderBy.Length > 0)
{{
    var dynamicOrderBy = $"" order by {{string.Join("", "", orderBy.Select((t)=> $""{{t.col}} {{(t.descending?""desc"":""asc"")}}"" ))}} "";
    var pattern = @""--\s*qforderby(.*--\s*endqforderby)?"";
    queryText = Regex.Replace(queryText, pattern, dynamicOrderBy, RegexOptions.IgnoreCase | RegexOptions.Singleline);
}}
// QfExpandoParams
{string.Join(n, fillExpandoParams)}
return queryText;
}}

public enum Cols
{{
{string.Join($",{n}", state._7ResultFields.Select((f) => $"{f.CSColumnName} = {f.ColumnOrdinal + 1}"))}
}}
";
            }
            else
            {
                return $@"public string getCommandText({state._8MethodSignature.Trim(spaceComma)}){{
var queryText = @""{state._6FinalQueryTextForCode}"";
// QfExpandoParams
{string.Join(n, fillExpandoParams)}
return queryText;
}}";
            }

        }
        public virtual string MakeTvpPocos(State state)
        {
            var pocos = new StringBuilder();
            foreach (var param in state._8QueryParams)
            {
                if (param.IsTableType)
                {
                    pocos.Append(makeAPoco(param));
                }
            }
            return pocos.ToString();
        }

        static string makeAPoco(QueryParamInfo param)
        {
            return
$@"public class {param.InnerCSType}{{
{string.Join("", param.ParamSchema.Select(col => $"public {col.CSType} {col.CSNamePascal}{{get; set;}}{n}"))}
}}
";

        }
        public virtual string MakeOtherMethods(State state)
        {
            var code = new StringBuilder();
            code.AppendLine($@"
protected void AddParameters({state._8MethodSignature}IDbCommand cmd)
{{"
);
            foreach (var qp in state._8QueryParams.Where(qp => !qp.IsQfExpandoParam))
            {
                if (qp.IsTableType)
                {
                    code.Append($@"
{{
var myParam = (SqlParameter)cmd.CreateParameter();
myParam.Direction = ParameterDirection.Input;
myParam.ParameterName = ""{qp.DbName}"";
myParam.SqlDbType = SqlDbType.Structured;
myParam.TypeName = ""{qp.DbType}"";
DataTable table = new DataTable();
using (var reader = ObjectReader.Create({qp.CSNameCamel}, new string[]{{""{string.Join("\",\"", qp.ParamSchema.Select(col => col.CSNamePascal ))}""}}))
{{
    table.Load(reader);
}}
myParam.Value = (object)table ?? DBNull.Value;

cmd.Parameters.Add(myParam);
}}");
                }
                else
                {
                    // Direction
                    string direction;
                    if (qp.IsInput && qp.IsOutput)
                        direction = "ParameterDirection.InputOutput";
                    else if (qp.IsOutput)
                        direction = "ParameterDirection.Output";
                    else
                        direction = "ParameterDirection.Input";


                    //code.AppendLine("AddAParameter(cmd, \"" + qp.DbType + "\", \"" + qp.DbName + "\", " + qp.CSName + ", " + qp.Length + ", " + qp.Scale + ", " + qp.Precision + ");");
                    code.Append($@"
{{
var myParam = cmd.CreateParameter();
myParam.Direction = {direction};
myParam.ParameterName = ""{qp.DbName}"";
myParam.DbType = (DbType)Enum.Parse(typeof(DbType), ""{qp.DbType}"");
{(qp.IsInput ? $"myParam.Value = (object){qp.CSNameCamel} ?? DBNull.Value;" : "") + n}
cmd.Parameters.Add(myParam);
}}"
                    );

                }
            }
                code.AppendLine("}");

            return code.ToString();
        }
        public virtual string CloseClass(State state)
        {
            return "}" + Environment.NewLine;
        }
        public virtual string CloseNamespace(State state)
        {
            if (!string.IsNullOrEmpty(state._4Namespace))
                return "}" + Environment.NewLine;
            else
                return "";
        }

        public static string MakeInterface(State state, bool includeAsync)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            code.AppendLine("public interface I" + state._4RepoClassName + "{" + Environment.NewLine);
            if (state._7ResultFields != null && state._7ResultFields.Count > 0)
            {
                code.AppendLine(
"List<" + state._4ResultInterfaceName + "> Execute(" + state._8MethodSignature.Trim(spaceComma) + @");
IEnumerable< " + state._4ResultInterfaceName + "> Execute(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
" + state._7ExecuteScalarReturnType + " ExecuteScalar(" + state._8MethodSignature.Trim(spaceComma) + @");
" + state._7ExecuteScalarReturnType + " ExecuteScalar(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
"
                );
                code.AppendLine(state._4ResultInterfaceName + @" Create(IDataRecord record);");

                // no GetOne() if output params
                if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
                {
                    code.AppendLine(
state._4ResultInterfaceName + " GetOne(" + state._8MethodSignature.Trim(spaceComma) + @");
" + state._4ResultInterfaceName + " GetOne(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);"
                    );
                }
                else code.AppendLine("// GetOne methods are not available because they do not play well with output params.");
            }
            code.AppendLine(
"int ExecuteNonQuery(" + state._8MethodSignature.Trim(spaceComma) + @");
int ExecuteNonQuery(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);"
            );
            if (includeAsync)
            {

                code.AppendLine("#region Async");

                if (state._7ResultFields != null && state._7ResultFields.Count > 0)
                {
                    code.AppendLine(
"Task<List<" + state._4ResultInterfaceName + ">> ExecuteAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<IEnumerable< " + state._4ResultInterfaceName + ">> ExecuteAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
" + "Task<" + state._7ExecuteScalarReturnType + "> ExecuteScalarAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
" + "Task<" + state._7ExecuteScalarReturnType + "> ExecuteScalarAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
"
                    );
                    // no GetOne() if output params
                    if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
                    {
                        code.AppendLine(
"Task<" + state._4ResultInterfaceName + "> GetOneAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<" + state._4ResultInterfaceName + "> GetOneAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);"
                        );
                    }
                    else code.AppendLine("// GetOne methods are not available because they do not play well with output params.");
                }
                code.AppendLine(
"Task<int> ExecuteNonQueryAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<int> ExecuteNonQueryAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);");
                code.AppendLine("# endregion");

            }
            code.AppendLine("}");
            return code.ToString();
        }

        public static string SelfTestUsings(State state)
            => $@"using Xunit;{n}";

        public static string MakeSelfTestMethod(State state)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            var nullCallingArgs = Regex.Replace(state._8CallingArgs, @"[\w|_|@]+", "null").Trim(spaceComma);
            code.Append($@"
[Fact]
public void {state._4RepoClassName}SelfTest()
{{
var queryText = getCommandText({nullCallingArgs});
// we'll be getting a runtime version with the comments section closed. To run without parameters, open it.
queryText = queryText.Replace(""/*designTime"", ""-- designTime"");
queryText = queryText.Replace(""endDesignTime*/"", ""-- endDesignTime"");
// QueryFirstConnectionFactory will be used, but we still need to reference a provider, for the prepare parameters method.
var schema = new AdoSchemaFetcher().GetFields(_connectionFactory.CreateConnection(), ""{state._3Config.Provider}"", queryText);
Assert.True({state._7ResultFields.Count} <=  schema.Count,
""Query only returns "" + schema.Count.ToString() + "" columns. Expected at least {state._7ResultFields.Count}."");
");
            for (int i = 0; i < state._7ResultFields.Count; i++)
            {
                var col = state._7ResultFields[i];
                code.Append("Assert.True(schema[" + i.ToString() + "].TypeDb == \"" + col.TypeDb + "\",");
                code.AppendLine("\"Result Column " + i.ToString() + " Type wrong. Expected " + col.TypeDb + ". Found \" + schema[" + i.ToString() + "].TypeDb + \".\");");
                code.Append("Assert.True(schema[" + i.ToString() + "].ColumnName == \"" + col.ColumnName + "\",");
                code.AppendLine("\"Result Column " + i.ToString() + " Name wrong. Expected " + col.ColumnName + ". Found \" + schema[" + i.ToString() + "].ColumnName + \".\");");
            }
            code.AppendLine("}");
            return code.ToString();
        }
    }
}
