using System.Collections.Generic;

namespace QueryFirst
{
    public interface IConfigBuilder
    {
        QfConfigModel GetInstallConfigForProjectType(List<ProjectSection> installFull, string projectType);
        QfConfigModel Resolve2Configs(QfConfigModel overridden, QfConfigModel overides);
    }
}