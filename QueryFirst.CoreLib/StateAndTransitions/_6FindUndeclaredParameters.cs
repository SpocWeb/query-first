using System;
using System.Text.RegularExpressions;

namespace QueryFirst
{
    public class _6FindUndeclaredParameters
    {
        private IProvider _provider;
        public _6FindUndeclaredParameters(IProvider provider)
        {
            _provider = provider;
        }
        public State Go(ref State state, out string outputMessage)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            // also called in the bowels of schema fetching, for Postgres, because no notion of declarations.
            var undeclared = _provider.FindUndeclaredParameters(state._5QueryAfterScaffolding, state._3Config.DefaultConnection, out outputMessage);
            state._6NewParamDeclarations = _provider.ConstructParameterDeclarations(undeclared);
            // with file watching, we must prevent an endless loop. Discovered parameters must be present for second pass.
            if (state._5QueryAfterScaffolding.Contains("-- endDesignTime"))
            {
                state._6QueryWithParamsAdded = state._5QueryAfterScaffolding.Replace("-- endDesignTime", state._6NewParamDeclarations + "-- endDesignTime");
            }
            else
            {
                var rn = Environment.NewLine;
                var m = Regex.Match(state._5QueryAfterScaffolding, @"\r\n?|\n");
                var endFirstLine = m.Groups[0].Index + m.Groups[0].Length;
                var firstLine = state._5QueryAfterScaffolding.Substring(0, endFirstLine);
                var restOfQuery = state._5QueryAfterScaffolding.Substring(endFirstLine);
                state._6QueryWithParamsAdded = $@"{firstLine}-- designTime
{state._6NewParamDeclarations}
-- endDesignTime
{restOfQuery}
";

            }
            state._6FinalQueryTextForCode = state._6QueryWithParamsAdded
                    .Replace("-- designTime", "/*designTime")
                    .Replace("-- endDesignTime", "endDesignTime*/")
                    // for inclusion in a verbatim string, only modif required is to double double quotes
                    .Replace("\"", "\"\"");

            state._6ProviderSpecificUsings = _provider.GetProviderSpecificUsings();

            return state;
        }
    }
}
