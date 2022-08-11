using QueryFirst.CoreLib.SchemaFetching.TypeMappings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QueryFirst.CoreLib.Generators
{
    [RegistrationName("TsInterfaceFromDto")]
    public class TypescriptInterfaceFromDto : IGenerator
    {
        static readonly string n = Environment.NewLine;
        public QfTextFile Generate(State state, Dictionary<string, string> options)
        {
            if (state._6FinalQueryTextForCode.ToLower().Contains("tsinterfacefromdto"))
            {
                var code =
    $@"export interface {state._1BaseName} {{
{string.Join($@",{n}", state._7ResultFields.Select(fld => $@"{fld.CamelCaseColumnName} : {SqlServer2Typescript.Map(fld.TypeDb)}{(fld.AllowDBNull ? "|null" : "")} // {fld.TypeDb ?? "unknown_DB_type"}{fld.ColumnSizeInBrackets} {(fld.AllowDBNull ? "nullable " : "not null")}"))}
}}";

                return new QfTextFile() { FileContents = code, Filename = GetFilename(state, options) };
            }
            return new QfTextFile() { Filename = GetFilename(state, options), DeleteMe = true };

        }
        /// <summary>
        /// In the output dir, we recreate the folder tree to avoid name collisions.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string GetFilename(State state, Dictionary<string, string> options)
            => Path.Combine(
               state._3Config.ProjectRoot,
               options.TryGetValue("outputDir", out string outputDir) ? outputDir : "",
               state._1CurrDir.Substring(state._3Config.ProjectRoot.Length + 1),
               state._1BaseName + ".ts"
           );
    }
}
