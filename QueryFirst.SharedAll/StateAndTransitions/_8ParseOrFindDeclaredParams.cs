using System;
using System.Text;
using System.Text.RegularExpressions;

namespace QueryFirst
{
    public class _8ParseOrFindDeclaredParams
    {
        IProvider _provider;
        public _8ParseOrFindDeclaredParams(IProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            _provider = provider;
        }
        /// <summary>
        /// Here, after the provider has recovered the final list of params, we're going to look at the sql
        /// and decide which, if any, are inside IN() clauses. These can be expanded at runtime from a list of 
        /// the detected/declared datatype. Why the hell not? QfExpandoParams !
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public State Go(ref State state)
        {
            var queryParams = _provider.ParseDeclaredParameters(state._6QueryWithParamsAdded, state._3Config.DefaultConnection);
            var fullSig = new StringBuilder();
            var inputOnlySig = new StringBuilder();
            var callSig = new StringBuilder();
            var inputOnlyCallSig = new StringBuilder();

            var pattern = @"--\s*designtime(.*--\s*enddesigntime)";
            var queryWoDesignTime = Regex.Replace(state._6FinalQueryTextForCode, pattern, "");

            foreach (var qp in queryParams)
            {
                string modifier;
                if (qp.IsInput && qp.IsOutput)
                {
                    modifier = "ref ";
                    fullSig.Append(modifier + qp.CSType + ' ' + qp.CSNameCamel + ", ");
                    callSig.Append(modifier + qp.CSNameCamel + ", ");
                }
                else if (qp.IsOutput)
                {
                    modifier = "out ";
                    fullSig.Append(modifier + qp.CSType + ' ' + qp.CSNameCamel + ", ");
                    callSig.Append(modifier + qp.CSNameCamel + ", ");
                }
                else
                {
                    modifier = "";
                    // QfExpandoParams only possible for regular input only params
                    var expandoPattern = $@"\sin\s*\(\s*\{qp.DbName}\s*\)";
                    if (!qp.IsTableType && Regex.IsMatch(queryWoDesignTime, expandoPattern, RegexOptions.IgnoreCase))
                    {
                        qp.IsQfExpandoParam = true;
                        fullSig.Append($@"List<{qp.CSType}> {qp.CSNameCamel},");
                        callSig.Append(qp.CSNameCamel + ", ");
                    }
                    else
                    {
                        fullSig.Append(modifier + qp.CSType + ' ' + qp.CSNameCamel + ", ");
                        callSig.Append(modifier + qp.CSNameCamel + ", ");
                    }

                }
                if (qp.IsTableType)
                    state._8HasTableValuedParams = true;


            }
            //signature trailing comma trimmed in place if needed. 

            state._8QueryParams = queryParams;
            state._8MethodSignature = fullSig.ToString() + state._5OrderByParamDeclarations;
            state._8CallingArgs = callSig.ToString() + state._5OrderByParamValues;
            //state._8InputOnlyCallingArgs = inputOnlyCallSig.ToString();
            //state._8InputOnlyMethodSignature = inputOnlySig.ToString();
            state._8HookupExecutionMessagesMethodText = _provider.HookUpForExecutionMessages();

            return state;
        }

    }
}
