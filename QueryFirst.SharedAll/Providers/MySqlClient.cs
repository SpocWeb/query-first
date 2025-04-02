using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryFirst.Providers
{
    [RegistrationName("MySql.Data.MySqlClient")]
    internal class MySqlClient : SqlClient
    {
        public static List<QueryParamInfo> ParseMySqlDeclaredParameters(string queryText)
        {
            var queryParams = new List<QueryParamInfo>();
            // get design time section
            var dt = Regex.Match(queryText, "-- designTime(?<designTime>.*)-- endDesignTime", RegexOptions.Singleline).Value;
            // extract declared parameters
            string pattern = @"\bset\s+(?<param>@\w+)((.*CAST\(.*AS\s+)(?<paramType>\w+))?";
            var myParams = Regex.Matches(dt, pattern, RegexOptions.IgnoreCase);
            if (myParams.Count > 0)
            {
                foreach (Match m in myParams)
                {
                    var name = m.Groups["param"].Value.Substring(1);
                    string dbType;
                    string csType;

                    // maps the cast-to type to a type in the DbType enum. 
                    // todo : finish this list and have some tests
                    switch (m.Groups["paramType"].Value.ToLower())
                    {
                        case "date":
                            dbType = "Date";
                            csType = "DateTime";
                            break;
                        case "datetime":
                        case "timestamp":
                            dbType = "DateTime";
                            csType = "DateTime";
                            break;
                        case "char":
                        case "varchar":
                            dbType = "String";
                            csType = "string";
                            break;
                        default:
                            dbType = "Object";
                            csType = "object";
                            break;
                    }

                    var qp = new QueryParamInfo
                    {
                        DbName = m.Groups["param"].Value,
                        CSNameCamel = char.ToLower(name.First()) + name.Substring(1),
                        CSNamePascal = char.ToUpper(name.First()) + name.Substring(1),
                        CSNamePrivate = "_" + char.ToLower(name.First()) + name.Substring(1),
                        CSType = csType,
                        DbType = dbType,
                        IsInput = true
                    };
                    // direction
                    // todo: parameter direction

                    queryParams.Add(qp);
                }
            }

            return queryParams;
        }

        private const string StrAddAParameter = @"private void AddAParameter(IDbCommand Cmd, string DbType, string DbName, object Value, int Length, int Scale, int Precision)
{
    ((MySql.Data.MySqlClient.MySqlCommand)Cmd).Parameters.AddWithValue(DbName, Value);
}";

        public override string MakeAddAParameter(State state) => StrAddAParameter;

        public override List<QueryParamInfo> FindUndeclaredParameters(string queryText, string connectionString, out string outputMessage)
        {
            outputMessage = null;
            return new List<QueryParamInfo>();
        }
        public override IDbConnection GetConnection(string connectionString) => new MySqlConnection(connectionString);

        public override List<QueryParamInfo> ParseDeclaredParameters(string queryText, string connectionString)
            => ParseMySqlDeclaredParameters(queryText);

        public override string HookUpForExecutionMessages() => "";
    }
}
