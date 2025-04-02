using System.Collections.Generic;

namespace QueryFirst
{
    /// <summary>
    /// Query parameters are used in the generated code to pass values to the query.
    /// They are also used to define the schema of the result set.
    /// </summary>
    public class QueryParamInfo
    {
        public string CSNameCamel { get; set; }

        public string CSNamePascal { get; set; }

        public string CSNamePrivate { get; set; }

        public string CSType { get; set; }

        /// <summary>
        /// For Table Valued Params, this is the poco that will supply a row.
        /// </summary>
        public string InnerCSType { get; set; }

        /// <summary>
        /// Table Valued Params, the inner cs type needs to be qualified with the class name.
        /// </summary>
        public string FullyQualifiedCSType { get; set; }

        public string DbName { get; set; }

        public string DbType { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool IsTableType { get; set; }

        public bool IsOutput { get; set; }

        public bool IsInput { get; internal set; }

        public bool IsQfExpandoParam { get; set; }

        public List<QueryParamInfo> ParamSchema { get; set; }
    }
}
