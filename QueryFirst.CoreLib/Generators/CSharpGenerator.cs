using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryFirst.Generators
{
    // Solution to the file nesting debacle: VS lets us add 1 extension, so all
    // the C# has to go in 1 file, that has to be called .sql.cs
    // Async can be a config option, sharing the same dto.
    // The class names can be suffixed to make clear what we're dealing with.
    [RegistrationName("CSharp")]
    public class CSharpGenerator : IGenerator
    {
        public string GetFilename(State state, Dictionary<string, string> options)
            => Path.Combine(
               state._3Config.ProjectRoot,
               options.TryGetValue("OutputDir", out string outputDir) ? outputDir : "",
               state._1CurrDir.Substring(state._3Config.ProjectRoot.Length + 1),
               state._1BaseName + ".sql.cs"
           );
        public QfTextFile Generate(State state, Dictionary<string, string> options)
        {
            bool generateAsync = false;
            _ = options.TryGetValue("generateAsync", out string generateAsyncStr) && bool.TryParse(generateAsyncStr, out generateAsync);
            StringBuilder Code = new StringBuilder();
            var _wrapper = new WrapperClassMaker();
            var _results = new ResultClassMaker();
            Code.Append(_wrapper.StartNamespace(state));
            Code.Append(_wrapper.Usings(state));
            if (state._3Config.MakeSelfTest.GetValueOrDefault())
                Code.Append(_wrapper.SelfTestUsings(state));
            if (state._7ResultFields != null && state._7ResultFields.Count > 0)
                Code.Append(_results.Usings());
            Code.Append(_wrapper.MakeInterface(state, generateAsync));
            Code.Append(_wrapper.StartClass(state));
            Code.AppendLine("");
            Code.AppendLine("#region Sync");
            Code.Append(_wrapper.MakeExecuteNonQueryWithoutConn(state));
            Code.Append(_wrapper.MakeExecuteNonQueryWithConn(state));
            Code.AppendLine("");
            Code.AppendLine("#endregion");
            Code.AppendLine("");
            if (generateAsync)
            {
                Code.AppendLine("");
                Code.AppendLine("#region ASync");
                Code.AppendLine("");
                Code.Append(_wrapper.MakeExecuteNonQueryWithoutConn_A(state));
                Code.Append(_wrapper.MakeExecuteNonQueryWithConn_A(state));
                Code.AppendLine("#endregion");
            }

            Code.Append(_wrapper.MakeGetCommandTextMethod(state));
            //Code.Append(_provider.MakeAddAParameter(state));
            Code.Append(_wrapper.MakeTvpPocos(state));

            if (state._3Config.MakeSelfTest.GetValueOrDefault())
                Code.Append(_wrapper.MakeSelfTestMethod(state));
            if (state._7ResultFields != null && state._7ResultFields.Count > 0)
            {
                Code.AppendLine("");
                Code.AppendLine("#region Sync");
                Code.AppendLine("");
                Code.Append(_wrapper.MakeExecuteWithoutConn(state));
                Code.Append(_wrapper.MakeExecuteWithConn(state));
                Code.Append(_wrapper.MakeGetOneWithoutConn(state));
                Code.Append(_wrapper.MakeGetOneWithConn(state));
                Code.Append(_wrapper.MakeExecuteScalarWithoutConn(state));
                Code.Append(_wrapper.MakeExecuteScalarWithConn(state));
                Code.AppendLine("");
                Code.AppendLine("#endregion");
                Code.AppendLine("");
                if (generateAsync)
                {
                    Code.AppendLine("#region ASync");
                    Code.AppendLine("");
                    Code.Append(_wrapper.MakeExecuteWithoutConn_A(state));
                    Code.Append(_wrapper.MakeExecuteWithConn_A(state));
                    Code.Append(_wrapper.MakeGetOneWithoutConn_A(state));
                    Code.Append(_wrapper.MakeGetOneWithConn_A(state));
                    Code.Append(_wrapper.MakeExecuteScalarWithoutConn_A(state));
                    Code.Append(_wrapper.MakeExecuteScalarWithConn_A(state));
                    Code.AppendLine("");
                    Code.AppendLine("#endregion");
                    Code.AppendLine("");
                }
                Code.Append(_wrapper.MakeCreateMethod(state));
            }
            Code.Append(_wrapper.MakeOtherMethods(state));
            Code.Append(_wrapper.CloseClass(state));

            if (state._7ResultFields != null && state._7ResultFields.Count > 0)
            {
                Code.Append(_results.StartClass(state));
                foreach (var fld in state._7ResultFields)
                {
                    Code.Append(_results.MakeProperty(fld));
                }
                Code.Append(_results.CloseClass());
            }
            Code.Append(_wrapper.CloseNamespace(state));

            return new QfTextFile()
            {
                Filename = GetFilename(state, options),
                FileContents = Code.ToString()
            };
        }
    }
}
