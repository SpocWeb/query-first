using System;

namespace QueryFirst.TypeMappings
{
    internal class Postgres2CS : IDB2CS
    {
        public string Map(string DBType, out string DBTypeNormalized, bool nullable = true)
        {
            throw new NotImplementedException();
        }
    }
}
