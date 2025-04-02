using System;

namespace QueryFirst.CoreLib.SchemaFetching.TypeMappings
{
    public static class SqlServer2Typescript
    {
        public static string Map(string inType)
        {
            switch (inType.ToLower())
            {
                case "bigint":
                    return "number";

                case "binary":
                case "image":
                case "timestamp":
                case "varbinary":
                    return "any";
                case "bit":
                    return "boolean";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                    // tricky one. In JSON on the wire they will be strings. Converting to a date somehow is left to intelligence of consumer.
                    return "Date";
                case "datetimeoffset":
                case "decimal":
                case "money":
                case "smallmoney":
                case "float":
                case "real":
                case "smallint":
                case "tinyint":
                case "int":
                    return "number";
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                case "uniqueidentifier":
                    return "string";
                case "xml":
                case "sql_variant":
                case "variant":
                case "udt":
                case "structured":
                    return "any";
                default:
                    throw new Exception("type not matched : " + inType);
                    // todo : keep going here. old method had a second switch on ResultFieldDetails.DataType to catch a bunch of never seen types

            }
        }
    }
}
