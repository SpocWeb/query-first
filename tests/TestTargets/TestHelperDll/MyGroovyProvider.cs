using QueryFirst;
using System.Data;

namespace TestHelperDll
{
    [RegistrationName("MyGroovyProvider")]
    public class TestProvider : QueryFirst.Providers.SqlClient
    {
        public string GetProviderSpecificUsings()
            => @"// hello from MyGroovyProvider
using System.Data.SqlClient;
using System.Threading.Tasks;
";
    }
}