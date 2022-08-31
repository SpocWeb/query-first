using QueryFirst;
using System.Data;

namespace TestHelperDll
{
    [RegistrationName("MyGroovyProvider")]
    public class TestProvider : QueryFirst.Providers.MicrosoftDataSqlClient
    {
        public override string HookUpForExecutionMessages()
        {
            return base.HookUpForExecutionMessages() + "// hello from MyGroovyProvider";
        }
    }
}