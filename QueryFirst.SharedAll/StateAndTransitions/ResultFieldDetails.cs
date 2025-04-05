using System.Collections.Generic;

namespace QueryFirst
{
    public class ResultFieldDetails
    {
        /// <summary>
        /// The raw column name, as it comes back in the result schema.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Column name safe for C#. If it starts with a number, we put an underscore.
        /// If the raw ColumnName has a query string, we take just the first part.
        /// </summary>
        public string CSColumnName { get; set; }
        /// <summary>
        /// The CSColumnName, with the first char lower case. That's all.
        /// </summary>
        public string CamelCaseColumnName { get; set; }
        public int ColumnOrdinal { get; set; }
        public int ColumnSize { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
        public bool IsUnique { get; set; }
        public string BaseColumnName { get; set; }
        public string BaseTableName { get; set; }
        public bool AllowDBNull { get; set; }
        public int ProviderType { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsAutoIncrement { get; set; }
        public bool IsRowVersion { get; set; }
        public bool IsLong { get; set; }
        public bool IsReadOnly { get; set; }
        public string ProviderSpecificDataType { get; set; }
        public string TypeDb { get; set; }
        public string TypeCs { get; set; }
        public string UdtAssemblyQualifiedName { get; set; }
        public int NewVersionedProviderType { get; set; }
        public bool IsColumnSet { get; set; }
        public int NonVersionedProviderType { get; set; }
        
        public string TypeCsShort => System2Alias.Map(TypeCs, AllowDBNull);

        public string ColumnSizeInBrackets => ColumnSize > 0 ? $"({ColumnSize})" : "";
    }
}
