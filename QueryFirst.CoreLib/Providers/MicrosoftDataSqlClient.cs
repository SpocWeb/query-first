using System;
using System.Collections.Generic;
using System.Text;

namespace QueryFirst.Providers
{
    [RegistrationName("Microsoft.Data.SqlClient")]
    internal class MicrosoftDataSqlClient : SqlClient
    {
        public override string GetProviderSpecificUsings() => "using Microsoft.Data.SqlClient;" + Environment.NewLine;
    }
}
