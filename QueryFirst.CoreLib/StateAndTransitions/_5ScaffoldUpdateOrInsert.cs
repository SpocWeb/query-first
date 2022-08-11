using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryFirst
{
    public class _5ScaffoldUpdateOrInsert
    {
        ISchemaFetcher _schemaFetcher;
        public _5ScaffoldUpdateOrInsert(ISchemaFetcher schemaFetcher)
        {
            _schemaFetcher = schemaFetcher;
        }
        public State Go(ref State state)
        {
            var matchInsert = Regex.Match(state._2InitialQueryText, @"^insert\s+into\s+(?<tableName>\w+)\.\.\.", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matchUpdate = Regex.Match(state._2InitialQueryText, @"^update\s+(?<tableName>\w+)\.\.\.", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (matchInsert.Success || matchUpdate.Success)
            {
                var targetTable = matchInsert.Success ? matchInsert.Groups["tableName"].Value : matchUpdate.Groups["tableName"].Value;
                // get schema
                var cols = _schemaFetcher.GetFields(
                    state._3Config.DefaultConnection,
                    state._3Config.Provider,
                    $"select * from {targetTable}"
                ).Where(col => (!col.IsIdentity && !(new string[] { "text", "ntext", "image", "timestamp" }).Contains(col.TypeDb) /* && !col.IsComputed*/));
                if (matchInsert.Success)
                {

                    state._5QueryAfterScaffolding =
    @"/* .sql query managed by QueryFirst add-in */


-- designTime - put parameter declarations and design time initialization here


-- endDesignTime
INSERT INTO " + targetTable + @" (
" + string.Join(",\r\n", cols.Select(col => col.ColumnName)) + @"
)
VALUES (
" + string.Join(",\r\n", cols.Select(col => "@" + col.ColumnName)) + @"
)

SELECT CAST(SCOPE_IDENTITY() AS INT) AS JustInsertedId";
                }
                else if (matchUpdate.Success)
                {
                    state._5QueryAfterScaffolding =
@"/* .sql query managed by QueryFirst add-in */


-- designTime - put parameter declarations and design time initialization here


-- endDesignTime
UPDATE " + targetTable + @"
SET 
" + string.Join(",\r\n", cols.Select(col => col.ColumnName + " = @" + col.ColumnName));
                }
            }
            else state._5QueryAfterScaffolding = state._2InitialQueryText;

            // Dynamic order by...
            if (Regex.Match(state._5QueryAfterScaffolding, @"--\s*qforderby", RegexOptions.IgnoreCase).Success)
            {
                state._5OrderByParamDeclarations = "(Cols col, bool descending)[] orderBy,";
                state._5OrderByParamValues = "orderBy,";
            }
            else
            {
                state._5OrderByParamDeclarations = "";
                state._5OrderByParamValues = "";
            }

            return state;
        }
    }
}
