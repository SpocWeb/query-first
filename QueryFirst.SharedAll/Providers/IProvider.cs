using System.Collections.Generic;
using System.Data;

namespace QueryFirst
{
    public interface IProvider
    {
        /// <summary> Harvests parameter declarations from the -- design time section. </summary>
        /// <remarks>
        /// Different DBs have different syntaxes for local variable declaration,
        /// and this method needs to understand your DBs syntax.
        ///  
        /// A SQL Server declaration for instance looks like `DECLARE @myLocalVariable [dataType]`
        /// MySql has `SET @myLocalVariable` (no datatype).
        /// Postgres has no local variables, so parameters need to be inferred directly from the text of the query.
        /// </remarks>
        List<QueryParamInfo> ParseDeclaredParameters(string queryText, string connectionString);

        /// <summary> Finds parameters in the query text that are not declared in the design time section. </summary>
        List<QueryParamInfo> FindUndeclaredParameters(string queryText, string connectionString, out string outputMessage);

        /// <summary>
        /// Find undeclared parameters and add them, either in the declarations section of the text (SqlServer, MySql)
        /// or as regular parameters on the command.
        /// </summary>
        /// <param name="cmd">The command to add parameters to. CommandText must already be assigned.</param>
        void PrepareParametersForSchemaFetching(IDbCommand cmd);
        string ConstructParameterDeclarations(List<QueryParamInfo> foundParams);

        /// <summary>
        /// Creates and returns a provider-specific connection instance.
        /// </summary>
        /// <param name="connectionString">Connection string for the connection.</param>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionString);

        /// <summary>
        /// Returns the C# type to which the reader result can be safely cast, and from which a sql parameter
        /// can be safely created.
        /// </summary>
        /// <param name="DBType">The Transact SQL type name.</param>
        /// <param name="DBTypeNormalized">Outputs the supplied DBType with capitalization corrected.</param>
        /// <returns>The C# type name.</returns>
        string TypeMapDB2CS(string DBType, out string DBTypeNormalized, bool nullable = true);

        /// <summary>
        /// 2022 Not used. Are other DBs still supported? 
        /// Generates the C# method that adds a parameter to the command.
        /// Called once for each parameter in the query.
        /// The method should have the signature...
        /// private void AddAParameter(IDbCommand Cmd, string DbType, string DbName, object Value, int Length)
        /// </summary>
        string MakeAddAParameter(State state);

        /// <summary>
        /// The ADO way is not working for Sql Server table valued functions.
        /// Here we give providers a chance to do better if the initial attempt returns no rows.
        /// </summary>
        List<ResultFieldDetails> GetQuerySchema2ndAttempt(string sql, string connectionString);

        /// <summary> Provider specific line in Execute methods to listen for execution messages. </summary>
        string HookUpForExecutionMessages();

        /// <summary>
        /// The generated code is DB-agnostic with one exception for the moment.
        /// We need to cast a System.Data.DBConnection to [System/Microsoft].Data.SqlClient
        /// to recover SqlInfoMessages. So we infer your prod db from your design time db, and
        /// put a using in the generated code to permit this cast.
        /// </summary>
        /// <returns></returns>
        string GetProviderSpecificUsings();
    }
}