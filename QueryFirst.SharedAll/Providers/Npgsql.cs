using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryFirst.Providers
{
    /// <summary> open-source .NET Data Provider for PostgreSQL </summary>
    [RegistrationName("Npgsql")]
    public class NpgSql : IProvider
    {
        private const string NpgSqlMakeAddAParameter =
            @"private void AddAParameter(IDbCommand Cmd, string DbType, string DbName, object Value, int Length, int Scale, int Precision)
{
    var myParam = new Npgsql.NpgsqlParameter();
    myParam.ParameterName = DbName;
    if(DbType != """")
    myParam.DbType = (DbType)System.Enum.Parse(typeof(DbType), DbType);
    myParam.Value = Value != null ? Value : DBNull.Value;
    Cmd.Parameters.Add(myParam);
}";

        public IDbConnection GetConnection(string connectionString) => new NpgsqlConnection(connectionString);

        public List<QueryParamInfo> ParseDeclaredParameters(string queryText, string connectionString)
            => NpgSqlExtensions.FindUndeclaredParameters(queryText);

        public string TypeMapDB2CS(string DBType, out string DBTypeNormalized, bool nullable = true)
            => NpgSqlExtensions._TypeMapDB2CS(DBType, out DBTypeNormalized, nullable);

        public string ConstructParameterDeclarations(List<QueryParamInfo> foundParams) => null;

        public virtual string MakeAddAParameter(State state) => NpgSqlMakeAddAParameter;

        public List<QueryParamInfo> FindUndeclaredParameters(string queryText, string connectionString,
            out string outputMessage)
        {
            outputMessage = null;
            return NpgSqlExtensions.FindUndeclaredParameters(queryText);
        }

        public void PrepareParametersForSchemaFetching(IDbCommand cmd)
        {
            // no notion of declaring parameters in Postgres
            // refactor, will this work harvesting connection string from passed command !
            foreach (var queryParam in NpgSqlExtensions.FindUndeclaredParameters(cmd.CommandText))
            {
                var myParam = new NpgsqlParameter
                {
                    ParameterName = queryParam.DbName
                };
                if (!string.IsNullOrEmpty(queryParam.DbType))
                {
                    myParam.DbType = (DbType)Enum.Parse(typeof(DbType), queryParam.DbType);
                }

                myParam.Value = DBNull.Value;
                cmd.Parameters.Add(myParam);
            }

        }

        /// <summary> No implementation for Postgres </summary>
        public List<ResultFieldDetails> GetQuerySchema2ndAttempt(string sql, string connectionString) => null;

        public string HookUpForExecutionMessages() => "";

        public string GetProviderSpecificUsings() => "";
    }

    public static class NpgSqlExtensions
    {

        private static readonly Dictionary<string, string> DbType2CsType = new Dictionary<string, string>()
        {
            {"Boolean","bool" },
            {"Int16","short" },
            {"Int32","int" },
            {"Int64","long" },
            {"Single","float" },
            {"Double","double" },
            {"Decimal","decimal" },
            {"VarNumeric","decimal" },
            {"Currency","decimal" },
            {"String","string" },
            {"StringFixedLength","string" },
            {"AnsiString","string" },
            {"AnsiStringFixedLength","string" },
            {"Date","DateTime" },
            {"DateTime","DateTime" },
            {"DateTimeOffset","DateTime" },
            {"Time","Timespan" },
            {"Binary","byte[]" }
        };

        public static List<QueryParamInfo> FindUndeclaredParameters(string queryText)
        {
            var queryParams = new List<QueryParamInfo>();
            var matchParams = Regex.Matches(queryText, "(:|@)\\w*");
            if (matchParams.Count <= 0)
            {
                return queryParams;
            }

            foreach (Match foundOne in matchParams)
            {
                string name;
                string userDeclaredType;
                var parts = foundOne.Value.Split('_');
                if (parts.Length > 1)
                {
                    userDeclaredType = parts[parts.Length - 1];
                    // just to verify. Does this throw?
                    _ = (DbType)Enum.Parse(typeof(DbType), userDeclaredType);
                    name = foundOne.Value.Substring(1, foundOne.Value.Length - userDeclaredType.Length - 2); // strip type to form csName
                }
                else
                {
                    name = foundOne.Value.Substring(1);
                    userDeclaredType = "";
                }
                var qp = TinyIoCContainer.Current.Resolve<QueryParamInfo>();
                qp.CSNameCamel = char.ToLower(name.First()) + name.Substring(1);
                qp.CSNamePascal = char.ToUpper(name.First()) + name.Substring(1);
                qp.CSNamePrivate = "_" + qp.CSNameCamel;
                qp.DbName = foundOne.Value;
                qp.DbType = userDeclaredType;
                qp.CSType = DbType2CsType.ContainsKey(userDeclaredType) ? DbType2CsType[userDeclaredType] : "object"; //lots of convertibility, will do till I figure out how NpgsqlTypes works. Not an enum. Doesn't have half the values listed in the table.
                queryParams.Add(qp);
            }
            return queryParams;
        }

        public static string _TypeMapDB2CS(string dbType, out string dbTypeNormalized, bool nullable = true)
        {
            // http://www.npgsql.org/doc/types.html
            switch (dbType.ToLower())
            {
                case "bool":
                case "boolean":
                    dbTypeNormalized = "Boolean";
                    return nullable ? "bool?" : "bool";
                case "int2":
                case "smallint":
                    dbTypeNormalized = "Smallint";
                    return nullable ? "short?" : "short";
                case "int4":
                case "integer":
                    dbTypeNormalized = "Integer";
                    return nullable ? "int?" : "int";
                case "int8":
                case "bigint":
                    dbTypeNormalized = "Bigint";
                    return nullable ? "long?" : "long";
                case "float4":
                case "real":
                    dbTypeNormalized = "Real";
                    return nullable ? "float?" : "float";
                case "float8":
                case "double":
                    dbTypeNormalized = "Double";
                    return nullable ? "double?" : "double";
                case "numeric":
                    dbTypeNormalized = "Numeric";
                    return nullable ? "decimal?" : "decimal";
                case "money":
                    dbTypeNormalized = "Money";
                    return nullable ? "decimal?" : "decimal";
                case "text":
                    dbTypeNormalized = "Text";
                    return "string";
                case "varchar":
                    dbTypeNormalized = "Varchar";
                    return "string";
                case "char":
                    dbTypeNormalized = "Char";
                    return "string";
                case "citext":
                    dbTypeNormalized = "Citext";
                    return "string";
                case "json":
                    dbTypeNormalized = "Json";
                    return "string";
                case "jsonb":
                    dbTypeNormalized = "Jsonb";
                    return "string";
                case "xml":
                    dbTypeNormalized = "Xml";
                    return "string";
                case "point":
                    dbTypeNormalized = "Point";
                    return "NpgsqlPoint";
                case "lseg":
                    dbTypeNormalized = "LSeg";
                    return "NpgsqlLSeg";
                case "path":
                    dbTypeNormalized = "Path";
                    return "NpgsqlPath";
                case "polygon":
                    dbTypeNormalized = "Polygon";
                    return "NpgsqlPolygon";
                default:
                    dbTypeNormalized = null;
                    return "object";
            }

            //case "":
            //    DBTypeNormalized = "";
            //    return nullable ? "?" : "";


            //Line line NpgsqlLine
            //Circle circle NpgsqlCircle
            //Box box NpgsqlBox
            //Bit bit BitArray or bool?
            //Varbit varbit BitArray or bool
            //Hstore hstore IDictionary< string,string>
            //Uuid uuid Guid
            //Cidr cidr IPAddress
            //Inet inet IPAddress
            //MacAddr macaddr PhysicalAddress
            //TsVector tsvector NpgsqlTsVector
            //Date date DateTime
            //Interval interval TimeSpan
            //Timestamp timestamp DateTime
            //TimestampTZ timestamptz DateTime
            //Time time TimeSpan
            //TimeTZ timetz DateTimeOffset
            //Bytea bytea byte[]
            //Oid oid uint
            //Xid xid uint
            //Cid cid uint
            //Oidvector oidvector uint[]
            //Name name string
            //InternalChar internalchar byte
            //Geometry geometry PostgisGeometry
        }
    }
}
