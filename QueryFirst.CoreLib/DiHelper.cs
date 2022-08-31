using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace QueryFirst
{
    public class DiHelper
    {
        public void RegisterAllQueries(IServiceCollection services, Assembly assembly)
        {
            var QfRepos = assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith("QfRepo") && t.IsClass)
                .OrderBy(t => t.Name)
                .ToList();
            var QfInterfaces = assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith("QfRepo") && t.IsInterface)
                .OrderBy(t => t.Name.Substring(1))
                .ToList();
            if (QfRepos.Count() != QfInterfaces.Count())
                throw new ApplicationException("Mismatch between classes and interfaces ending QfRepo.");
            for (int i = 0; i < QfRepos.Count(); i++)
            {
                services.Add(new ServiceDescriptor(
                    QfInterfaces[i],
                    QfRepos[i],
                    ServiceLifetime.Singleton
                ));
            }
        }
    }
}
