using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryFirst.Generators
{
    public partial class WrapperClassMaker
    {
        char[] spaceComma = new char[] { ',', ' ' };
        public virtual string MakeExecuteWithoutConn_A(State state)
        {
            StringBuilder code = new StringBuilder();
            char[] spaceComma = new char[] { ',', ' ' };
            // Execute method, without connection
            code.AppendLine( $@"
public static async Task<List<{state._4ResultInterfaceName}>> ExecuteStaticAsync({state._8MethodSignature.Trim(spaceComma)})
=> await inst.ExecuteAsync({state._8CallingArgs});
public virtual async Task<List<{state._4ResultInterfaceName}>> ExecuteAsync({state._8MethodSignature.Trim(spaceComma)})
{{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{{
await conn.OpenAsync();
var returnVal = await ExecuteAsync({state._8CallingArgs}conn);
/*
{string.Join($";{n}", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNameCamel} = {qp.CSNamePascal}"))}
*/
return returnVal.ToList();
}}
}}");

            return code.ToString();
        }
        public virtual string MakeExecuteWithConn_A(State state)
        {
            StringBuilder code = new StringBuilder();
            // Execute method with connection. Use properties. First variant assigns props, then calls inner exec with no args.
            code.AppendLine($@"
public static async Task<IEnumerable<{state._4ResultInterfaceName}>> ExecuteStaticAsync({state._8MethodSignature} IDbConnection conn, IDbTransaction tx = null)
=> await inst.ExecuteAsync({state._8CallingArgs}conn, tx);

public virtual async Task<IEnumerable<{state._4ResultInterfaceName}>> ExecuteAsync({state._8MethodSignature} IDbConnection conn, IDbTransaction tx = null){{
{state._8HookupExecutionMessagesMethodText}
using (DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters({state._8CallingArgs} cmd);
SqlDataReader reader = (SqlDataReader)await cmd.ExecuteReaderAsync();
                

// Assign output parameters to instance properties. These will be available after this method returns.
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput)
.Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;\r\n"))}
*/

return ReadItems(reader).ToArray();
}}
}}
IEnumerable<{state._4ResultInterfaceName}> ReadItems(SqlDataReader reader)
{{
    while (reader.Read())
    {{
        yield return Create(reader);
    }}
}}

");


            return code.ToString();
        }
        public virtual string MakeGetOneWithoutConn_A(State state)
        {
            string code = "";
            // don't make GetOne if there are output params.
            if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
            {
                code += $@"
public static async Task<{state._4ResultInterfaceName}> GetOneStaticAsync({state._8MethodSignature.Trim(spaceComma)})
=> await inst.GetOneAsync({state._8CallingArgs.Trim(spaceComma)});

public virtual async Task<{state._4ResultInterfaceName}> GetOneAsync({state._8MethodSignature.Trim(spaceComma)})
{{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{{
    await conn.OpenAsync();
    return await GetOneAsync({state._8CallingArgs} conn);
}}
}}
";
            }
            return code;
        }
        public virtual string MakeGetOneWithConn_A(State state)
        {
            string code = "";
            // don't make GetOne if there are output params.
            if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
            {
                // GetOne() with connection

                code += $@"
public static async Task<{state._4ResultInterfaceName}> GetOneStaticAsync({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> await inst.GetOneAsync({state._8CallingArgs}conn, tx );

public virtual async Task<{state._4ResultInterfaceName}> GetOneAsync({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{
{state._8HookupExecutionMessagesMethodText}
var all = await ExecuteAsync({state._8CallingArgs} conn,tx);
{state._4ResultInterfaceName} returnVal;
using (IEnumerator<{state._4ResultInterfaceName}> iter = all.GetEnumerator())
{{
iter.MoveNext();
returnVal = iter.Current;
}}
return returnVal;
}}
";
            }
            return code;
        }
        public virtual string MakeExecuteScalarWithoutConn_A(State state)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            StringBuilder code = new StringBuilder();
            //ExecuteScalar without connection
            code.AppendLine($@"
public static async Task<{state._7ExecuteScalarReturnType}> ExecuteScalarStaticAsync({state._8MethodSignature.Trim(spaceComma)})
=> await inst.ExecuteScalarAsync({state._8CallingArgs});

public virtual async Task<{state._7ExecuteScalarReturnType}> ExecuteScalarAsync({state._8MethodSignature.Trim(spaceComma)})
{{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{{
conn.Open();
var returnVal = await ExecuteScalarAsync({state._8CallingArgs} conn);
/*
{string.Join($";{n}", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNameCamel} = {qp.CSNamePascal}"))};
*/
return returnVal;
}}
}}
");

            return code.ToString();
        }
        public virtual string MakeExecuteScalarWithConn_A(State state)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteScalar() with connection
            code.AppendLine($@"
public static async Task<{state._7ExecuteScalarReturnType}> ExecuteScalarStaticAsync({state._8MethodSignature} IDbConnection conn, IDbTransaction tx = null)
=> await inst.ExecuteScalarAsync({state._8CallingArgs}conn, tx);

public virtual async Task<{state._7ExecuteScalarReturnType}> ExecuteScalarAsync({state._8MethodSignature} IDbConnection conn, IDbTransaction tx = null)
{{
using(DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters({state._8CallingArgs} cmd);
var result = await cmd.ExecuteScalarAsync();

// only convert dbnull if nullable
// Assign output parameters to instance properties. 
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput)
.Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;{n}"))}
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
        public virtual string MakeExecuteNonQueryWithoutConn_A(State state)
        {
            char[] spaceComma = new char[] { ',', ' ' };
            //ExecuteScalar without connection
            string code = "";
            code += $@"
public static async Task<int> ExecuteNonQueryStaticAsync({state._8MethodSignature.Trim(spaceComma)})
=> await inst.ExecuteNonQueryAsync({state._8CallingArgs.Trim(spaceComma)});

public virtual async Task<int> ExecuteNonQueryAsync({state._8MethodSignature.Trim(spaceComma)})
{{
using (DbConnection conn = (DbConnection)_connectionFactory.CreateConnection())
{{
conn.Open();
var returnVal = await ExecuteNonQueryAsync({state._8CallingArgs} conn);
/*
{string.Join($";{n}", state._8QueryParams.Where(qp => qp.IsOutput).Select(qp => $"{qp.CSNameCamel} = {qp.CSNamePascal}"))};
*/
return returnVal;
}}
}}
";
            return code;
        }
        public virtual string MakeExecuteNonQueryWithConn_A(State state)
        {
            StringBuilder code = new StringBuilder();
            // ExecuteNonQuery() with connection
            code.AppendLine($@"
public static async Task<int> ExecuteNonQueryStaticAsync({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
=> await inst.ExecuteNonQueryAsync({state._8CallingArgs}conn, tx);

public virtual async Task<int> ExecuteNonQueryAsync({state._8MethodSignature}IDbConnection conn, IDbTransaction tx = null)
{{
{state._8HookupExecutionMessagesMethodText}
using(DbCommand cmd = ((SqlConnection)conn).CreateCommand())
{{
if(tx != null)
cmd.Transaction = (DbTransaction)tx;

cmd.CommandText = getCommandText();
AddParameters({state._8CallingArgs} cmd);
var result = await cmd.ExecuteNonQueryAsync();

// Assign output parameters to instance properties. 
/*
{string.Join("", state._8QueryParams.Where(qp => qp.IsOutput)
.Select(qp => $"{qp.CSNamePascal} = ((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value == DBNull.Value?null:({qp.CSType})((SqlParameter)cmd.Parameters[\"{qp.DbName}\"]).Value;\r\n"))}
*/
// only convert dbnull if nullable
return result;
}}
}}
"
            );// close ExecuteNonQuery()
            return code.ToString();
        }

        public string MakeAsyncMethodListForInterfaceDefinition(State state)
        {
            StringBuilder code = new StringBuilder();
            if (state._7ResultFields != null && state._7ResultFields.Count > 0)
            {
                code.AppendLine(
"Task<List<" + state._4ResultInterfaceName + ">> ExecuteAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<IEnumerable< " + state._4ResultInterfaceName + ">> ExecuteAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
" + "Task<" + state._7ExecuteScalarReturnType + "> ExecuteScalarAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
" + "Task<" + state._7ExecuteScalarReturnType + "> ExecuteScalarAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);
"
                );
                code.AppendLine(
@"Task<List< " + state._4ResultInterfaceName + @" >> ExecuteAsync();
Task<IEnumerable<" + state._4ResultInterfaceName + @">> ExecuteAsync(IDbConnection conn, IDbTransaction tx = null);
Task<" + state._7ExecuteScalarReturnType + @"> ExecuteScalarAsync();
Task<" + state._7ExecuteScalarReturnType + @"> ExecuteScalarAsync(IDbConnection conn, IDbTransaction tx = null);");

                if (state._8QueryParams.Where(qp => qp.IsOutput).Count() == 0)
                {
                    code.AppendLine(
"Task<" + state._4ResultInterfaceName + "> GetOneAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<" + state._4ResultInterfaceName + "> GetOneAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);"
                    );
                    code.AppendLine(
"Task<" + state._4ResultInterfaceName + @"> GetOneAsync();
Task<" + state._4ResultInterfaceName + @"> GetOneAsync(IDbConnection conn, IDbTransaction tx = null);"
                    );
                }
                else code.AppendLine("// GetOne methods are not available because they do not play well with output params.");
            }
            code.AppendLine(
"Task<int> ExecuteNonQueryAsync(" + state._8MethodSignature.Trim(spaceComma) + @");
Task<int> ExecuteNonQueryAsync(" + state._8MethodSignature + @"IDbConnection conn, IDbTransaction tx = null);");
            code.AppendLine(
@"Task<int> ExecuteNonQueryAsync();            
Task<int> ExecuteNonQueryAsync(IDbConnection conn, IDbTransaction tx = null);
");

            return code.ToString();
        }
    }
}
