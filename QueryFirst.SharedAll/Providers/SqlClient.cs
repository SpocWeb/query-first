using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if VS16
#else
using SqlConnection = Microsoft.Data.SqlClient.SqlConnection;
#endif

namespace QueryFirst.Providers
{
    [RegistrationName("System.Data.SqlClient")]
    public class SqlClient : IProvider
    {
        private const string SqlServerHookUpForExecutionMessages =
            @"// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
    delegate (object sender, SqlInfoMessageEventArgs e)  { AppendExececutionMessage(e.Message); });";

        private const string SqlServerUsings = @"using System.Data.SqlClient;
using System.Threading.Tasks;
";

        public virtual string HookUpForExecutionMessages() => SqlServerHookUpForExecutionMessages;

        public virtual string GetProviderSpecificUsings() => SqlServerUsings;

        public virtual IDbConnection GetConnection(string connectionString) => new SqlConnection(connectionString);

        public virtual List<QueryParamInfo> ParseDeclaredParameters(string queryText, string connectionString)
            => new SqlConnection(connectionString)._ParseDeclaredParameters(queryText);

        public virtual string TypeMapDB2CS(string DBType, out string DBTypeNormalized, bool nullable = true) =>
            SqlServerExtensions._TypeMapDB2CS(DBType, out DBTypeNormalized, nullable);

        public virtual string MakeAddAParameter(State state) => SqlServerExtensions.MakeAddAParameter(state);

        public List<ResultFieldDetails> GetQuerySchema2ndAttempt(string sql, string connectionString)
        {
            using (var conn = GetConnection(connectionString))
                return conn.GetQuerySchema2ndAttempt(sql);
        }

        public virtual void PrepareParametersForSchemaFetching(IDbCommand cmd)
        {
            // nothing to do here.
        }

        public virtual List<QueryParamInfo> FindUndeclaredParameters(string queryText, string connectionString,
            out string outputMessage)
        {
            using (var conn = GetConnection(connectionString))
                return SqlServerExtensions._FindUndeclaredParameters(conn, queryText, out outputMessage);
        }

        public string ConstructParameterDeclarations(List<QueryParamInfo> foundParams)
            => SqlServerExtensions._ConstructParameterDeclarations(foundParams);
    }

    public static class SqlServerExtensions
    {
        public static List<QueryParamInfo> _ParseDeclaredParameters(this SqlConnection conn, string queryText)
        {
            var textParams = ExtractParamsFromText(queryText);
            var queryParams = new List<QueryParamInfo>();
            foreach (var para in textParams)
            {
                QueryParamInfo qp;
                try
                {
                    qp = GetParamInfo(para.SqlName, para.SqlTypeAndLength);
                }
                catch (TypeNotMatchedException)
                {
                    try
                    {
                        qp = GetParamInfoSecondAttempt(conn, para.SqlTypeAndLength, para.SqlName);
                    }
                    catch (Exception ex)
                    {
                        throw new TypeNotMatchedException("Unable to find a type for " + para.SqlTypeAndLength, ex);
                    }
                }
                // direction
                if ((para.Direction ?? "").ToLower() == "--inout")
                {
                    qp.IsInput = true;
                    qp.IsOutput = true;
                }
                else if ((para.Direction ?? "").ToLower() == "--output")
                    qp.IsOutput = true;
                else
                    qp.IsInput = true;

                queryParams.Add(qp);
            }
            return queryParams;
        }


        public static List<ParamFromText> ExtractParamsFromText(string queryText)
        {
            var queryParams = new List<ParamFromText>();
            // get design time section
            var dt = Regex.Match(queryText, "-- designTime(?<designTime>.*)-- endDesignTime", RegexOptions.Singleline).Value;
            // extract declared parameters
            string pattern = @"declare\s*@(?'name'\S*)\s?(?'sqlType'[^;]*)[\s;]*(?'restOfLine'\S*)\s*$";
            Match m = Regex.Match(dt, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            while (m.Success)
            {
                queryParams.Add(new ParamFromText
                {
                    SqlName = m.Groups["name"].Value,
                    SqlTypeAndLength = m.Groups["sqlType"].Value,
                    Direction = m.Groups["restOfLine"].Value
                });
                m = m.NextMatch();
            }
            return queryParams;
        }

        static QueryParamInfo GetParamInfo(string name, string sqlTypeAndLength)
        {
            var qp = new QueryParamInfo();
            var m = Regex.Match(sqlTypeAndLength, @"(?'type'^\w*)\(?(?'firstNum'\d*),?(?'secondNum'\d*)");
            var typeOnly = m.Groups["type"].Value;
            int.TryParse(m.Groups["firstNum"].Value, out int firstNum);
            int.TryParse(m.Groups["secondNum"].Value, out int secondNum);
            if (secondNum != 0)
            {
                qp.Precision = firstNum;
                qp.Scale = secondNum;
            }
            else if (typeOnly.ToLower() == "datetime2")
            {
                qp.Precision = firstNum;
            }
            else if (firstNum > 0)
            {
                qp.Length = firstNum;
            }
            //string normalizedType;
            // we have no info for the nullability of query params, so we'll make them all nullable
            //var csType = System2Alias.Map( TypeMapDB2CS(typeOnly, out normalizedType), true );

            try
            {
                // hack
                if (typeOnly == "sql_variant")
                    typeOnly = "Variant";
                var sqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), typeOnly, true);
                var csType = TypeConvertor.ToNetType(sqlDbType);
                var dbType = TypeConvertor.ToDbType(sqlDbType);
                qp.CSType = System2Alias.Map(csType.FullName, true); // will look up aliases of system types, and append the question mark.
                qp.FullyQualifiedCSType = csType.FullName;
                qp.DbType = dbType.ToString();
                qp.CSNameCamel = char.ToLower(name.First()) + name.Substring(1);
                qp.CSNamePascal = char.ToUpper(name.First()) + name.Substring(1);
                qp.CSNamePrivate = "_" + qp.CSNameCamel;
                qp.DbName = '@' + name;
                return qp;
            }
            catch (Exception ex)
            {
                throw new TypeNotMatchedException(ex.Message);
            }

        }

        /// <summary>
        /// First attempt matches system types using local type map.
        /// Here we're going to deal with user defined table types as input parameters.
        /// </summary>
        private static QueryParamInfo GetParamInfoSecondAttempt(this SqlConnection conn, string paramType, string paramName)
        {
            // Table Valued Parameters...
            var s = new Server(new ServerConnection(conn, null));
            var myDb = s.Databases.Cast<Database>().Where(db => db.Name == conn.Database).FirstOrDefault();
            var type = myDb.UserDefinedTableTypes.Cast<UserDefinedTableType>().Where(t => t.Name == paramType);

            if (type.Count() <= 0)
            {
                throw new TypeNotMatchedException("No user defined type to match " + paramType);
            }

            var returnVal = new QueryParamInfo
            {
                CSNameCamel = paramName.First().ToString().ToLower() + paramName.Substring(1),
                CSNamePascal = paramName.First().ToString().ToUpper() + paramName.Substring(1),
                DbName = "@" + paramName,
                //DbType = type.First().Urn.Type,
                DbType = paramType,
                CSType = $"IEnumerable<{paramType.First().ToString().ToUpper() + paramType.Substring(1)}>",
                InnerCSType = paramType.First().ToString().ToUpper() + paramType.Substring(1),
                IsTableType = true,
                ParamSchema = new List<QueryParamInfo>()
            };
            foreach (Column col in type.First().Columns)
            {
                string normalizedType;
                var csType = _TypeMapDB2CS(col.DataType.Name, out normalizedType);

                returnVal.ParamSchema.Add(new QueryParamInfo
                {
                    CSNameCamel = char.ToLower(col.Name.First()) + col.Name.Substring(1),
                    CSNamePascal = char.ToUpper(col.Name.First()) + col.Name.Substring(1),
                    DbType = normalizedType,
                    CSType = csType,
                    DbName = '@' + col.Name

                });
            }
            return returnVal;

        }

        public static List<QueryParamInfo> _FindUndeclaredParameters(this IDbConnection conn, string queryText, out string outputMessage)
        {
            outputMessage = null;
            var myParams = new List<QueryParamInfo>();
            // sp_describe_undeclared_parameters
            try
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = "sp_describe_undeclared_parameters @tsql";
                var tsql = new SqlParameter("@tsql", SqlDbType.NChar)
                {
                    Value = queryText
                };
                cmd.Parameters.Add(tsql);

                conn.Open();
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    // ignore global variables
                    if (rdr.GetString(1).Substring(0, 2) != "@@")
                    {
                        // build declaration.
                        myParams.Add(new QueryParamInfo()
                        {
                            DbName = rdr.GetString(1),
                            DbType = rdr.GetString(3),
                            Length = rdr.GetInt16(4),
                            IsInput = rdr.GetBoolean(19),
                            IsOutput = rdr.GetBoolean(20)
                        }
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                outputMessage = $"Unable to find undeclared parameters. ({ex.Message}) Make sure your parameters are declared in the designTime section\n";
            }
            return myParams;
        }
        public static string _ConstructParameterDeclarations(List<QueryParamInfo> foundParams)
        {
            StringBuilder bldr = new StringBuilder();

            foreach (var qp in foundParams)
            {
                string direction;
                if (qp.IsInput && qp.IsOutput)
                    direction = " --InOut";
                else if (qp.IsOutput)
                    direction = " --Output";
                else
                    direction = "";

                // build declaration.
                bldr.Append("declare " + qp.DbName + " " + qp.DbType);
                //if (qp.Length != 0)
                //{
                //    bldr.Append("(" + qp.Length + ")");
                //}
                bldr.Append($";{direction}\r\n");
            }
            return bldr.ToString();
        }
        public static string MakeAddAParameter(State state)
        {
            StringBuilder code = new StringBuilder();
            code.Append(@"
            private void AddAParameter(IDbCommand Cmd, string DbType, string DbName, object Value, int Length, byte Scale, byte Precision)
            {
            var dbType = (SqlDbType)System.Enum.Parse(typeof(SqlDbType), DbType);
            SqlParameter myParam;
            if(Length != 0){
            myParam = new SqlParameter(DbName, dbType, Length);
            }else{
            myParam = new SqlParameter(DbName, dbType);
            }");

            // For table valued parameters, convert IEnumerable to DataTable
            // The following method has no dynamic parts, but since it has a reference to FastMember, we only want it if it's needed
            if (state._8HasTableValuedParams)
            {
                code.Append(@"
                    if (DbType == ""UserDefinedTableType"")
                    {
                        DataTable table = new DataTable();
                        using (var reader = ObjectReader.Create((IEnumerable<GenericAccountNumType>)Value))
                        {
                            table.Load(reader);
                        }
                        Value = table;
                    }
                ");

            }
            code.Append(@"
            myParam.Value = Value != null ? Value : DBNull.Value;
            myParam.Scale = Scale;
            myParam.Precision = Precision;
            Cmd.Parameters.Add( myParam);
            }
            ");

            return code.ToString();
        }
        public static string _TypeMapDB2CS(string DBType, out string DBTypeNormalized, bool nullable = true)
        {
            switch (DBType.ToLower())
            {
                case "bigint":
                    DBTypeNormalized = "BigInt";
                    return nullable ? "System.Int64?" : "System.Int64";
                case "binary":
                    DBTypeNormalized = "Binary";
                    return "System.Byte[]";
                case "image":
                    DBTypeNormalized = "Image";
                    return "System.Byte[]";
                case "timestamp":
                    DBTypeNormalized = "Timestamp";
                    return "System.Byte[]";
                case "varbinary":
                    DBTypeNormalized = "Varbinary";
                    return "System.Byte[]";
                case "bit":
                    DBTypeNormalized = "Bit";
                    return nullable ? "System.Boolean?" : "System.Boolean";
                case "date":
                    DBTypeNormalized = "Date";
                    return nullable ? "DateTime?" : "DateTime";
                case "datetime":
                    DBTypeNormalized = "DateTime";
                    return nullable ? "DateTime?" : "DateTime";
                case "datetime2":
                    DBTypeNormalized = "DateTime2";
                    return nullable ? "DateTime?" : "DateTime";
                case "smalldatetime":
                    DBTypeNormalized = "SmallDateTime";
                    return nullable ? "DateTime?" : "DateTime";
                case "time":
                    DBTypeNormalized = "Time";
                    return nullable ? "TimeSpan?" : "TimeSpan";
                case "datetimeoffset":
                    DBTypeNormalized = "DateTimeOffset";
                    return nullable ? "DateTimeOffset?" : "DateTimeOffset";
                case "decimal":
                    DBTypeNormalized = "Decimal";
                    return nullable ? "System.Decimal?" : "System.Decimal";
                case "money":
                    DBTypeNormalized = "Money";
                    return nullable ? "System.Decimal?" : "System.Decimal";
                case "smallmoney":
                    DBTypeNormalized = "SmallMoney";
                    return nullable ? "System.Decimal?" : "System.Decimal";
                case "float":
                    DBTypeNormalized = "Float";
                    return nullable ? "System.Double?" : "System.Double";
                case "real":
                    DBTypeNormalized = "Real";
                    return nullable ? "System.Single?" : "System.Single";
                case "smallint":
                    DBTypeNormalized = "SmallInt";
                    return nullable ? "System.Single?" : "System.Single";
                case "tinyint":
                    DBTypeNormalized = "TinyInt";
                    return nullable ? "System.Byte?" : "System.Byte";
                case "int":
                    DBTypeNormalized = "Int";
                    return nullable ? "System.Int32?" : "System.Int32";
                case "char":
                    DBTypeNormalized = "Char";
                    return "System.String";
                case "nchar":
                    DBTypeNormalized = "NChar";
                    return "System.String";
                case "ntext":
                    DBTypeNormalized = "NText";
                    return "System.String";
                case "nvarchar":
                    DBTypeNormalized = "NVarChar";
                    return "System.String";
                case "varchar":
                    DBTypeNormalized = "VarChar";
                    return "System.String";
                case "text":
                    DBTypeNormalized = "Text";
                    return "System.String";
                case "xml":
                    DBTypeNormalized = "Xml";
                    return "System.String";
                case "sql_variant":
                    DBTypeNormalized = "Variant";
                    return "System.Object";
                case "variant":
                    DBTypeNormalized = "Variant";
                    return "System.Object";
                case "udt":
                    DBTypeNormalized = "Udt";
                    return "System.Object";
                case "structured":
                    DBTypeNormalized = "Structured";
                    return "DataTable";
                case "uniqueidentifier":
                    DBTypeNormalized = "UniqueIdentifier";
                    return nullable ? "Guid?" : "Guid";
                default:
                    throw new TypeNotMatchedException("type not matched : " + DBType);
                    // todo : keep going here. old method had a second switch on ResultFieldDetails.DataType to catch a bunch of never seen types

            }
        }

        public static List<ResultFieldDetails> GetQuerySchema2ndAttempt(this IDbConnection conn, string sql)
        {
            var command = conn.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_DESCRIBE_FIRST_RESULT_SET";

            var tsql = new SqlParameter("@TSQL", sql);
            tsql.Direction = ParameterDirection.Input;
            command.Parameters.Add(tsql);
            var returnVal = new List<ResultFieldDetails>();
            using (var dt = new DataTable()) 
            {
                conn.Open();
                var dr = command.ExecuteReader();
                dt.Load(dr);
                foreach (DataRow rec in dt.Rows)
                {
                    string colName = rec.Field<string>("name");
                    string csColName;
                    if (Regex.Match((colName.Substring(0, 1)), "[0-9]").Success)
                        csColName = "_" + colName;
                    else
                        csColName = colName;
                    var qp = GetParamInfo("dontCare", rec.Field<string>("system_type_name"));
                    returnVal.Add(new ResultFieldDetails
                    {
                        ColumnName = colName,
                        AllowDBNull = rec.Field<bool?>("is_nullable").GetValueOrDefault(),
                        BaseColumnName = rec.Field<string>("name"),
                        ColumnOrdinal = rec.Field<int>("column_ordinal"),
                        CSColumnName = csColName,
                        IsIdentity = rec.Field<bool?>("is_identity_column").GetValueOrDefault(),
                        NumericPrecision = qp.Precision,
                        NumericScale = qp.Scale,
                        TypeCs = qp.FullyQualifiedCSType,
                    }
                    );
                }
            }
            return returnVal;
        }

        public class ParamFromText
        {
            public string SqlName { get; set; }
            public string SqlTypeAndLength { get; set; }
            public string Direction { get; set; }
        }
    }

    public class TypeNotMatchedException : Exception
    {
        public TypeNotMatchedException(string message) : base(message) { }
        public TypeNotMatchedException(string message, Exception ex) : base(message, ex) { }
    }

}
