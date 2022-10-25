using System.Collections.Generic;

namespace QueryFirst
{
    public class QfConfigModel
    {
        public string DefaultConnection { get; set; }
        public string Provider { get; set; }
        public List<string> HelperAssemblies { get; set; }
        public bool? MakeSelfTest { get; set; }
        public List<Generator> Generators { get; set; }
        public string Namespace { get; set; }
        public string ResultClassName { get; set; }
        /// <summary>
        /// Deprecated? We can deal with this in subclasses. Take this out?        /// 
        /// </summary>
        public string ResultInterfaceName { get; set; }
        /// <summary>
        /// The folder containing qfconfig.json. Will be used as the project root for
        /// deciding where to put Typescript files, for example.
        /// </summary>
        public string ProjectRoot { get; set; }
        /// <summary>
        /// As defined in RootNamespace element of the csproj if present,
        /// otherwise the filename of the .csproj.
        /// </summary>
        public string ProjectNamespace { get; set; }
        /// <summary>
        /// Suffix appended to the base query name to get the C# repository name.
        /// Default "QfRepo" in install config
        /// </summary>
        public string RepoSuffix { get; set; }
        /// <summary>
        /// Suffix appended to the base query name to get the C# DTO name.
        /// Default "QfDto" in install config
        /// </summary>
        public string DtoSuffix { get; set; }

    }
}
