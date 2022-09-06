// You will need a COM reference to QueryFirst.dll
using QueryFirst;
using QueryFirst.Providers;

namespace TestHelperDll
{
    [RegistrationName("MyGroovyProvider")]
    public class TestProvider : MicrosoftDataSqlClient
    {
        public override string HookUpForExecutionMessages()
        {
            return base.HookUpForExecutionMessages() + "// hello from MyGroovyProvider";
        }
    }
}