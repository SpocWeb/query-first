using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryFirst
{
    /// <summary>
    /// Accesses DI, Doesn't interact with filesystem, could be a numbered transition but for the moment we aren't adding files to the state object.
    /// There's no reason not to?
    /// </summary>
    public class InstantiateAndCallGenerators
    {
        public IQfTextFileWriter FileWriter { get; set; }
        private TinyIoCContainer _tiny = TinyIoCContainer.Current;
        public IEnumerable<QfTextFile> Go(State state)
        {
            var returnVal = new List<QfTextFile>();
            foreach (var genConfig in state._3Config.Generators)
            {
                if (_tiny.CanResolve<IGenerator>(genConfig.Name))
                {
                    var generator = _tiny.Resolve<IGenerator>(genConfig.Name);
                    returnVal.Add(generator.Generate(state, genConfig.Options));
                }
                else
                {
                    Console.WriteLine($"Cannot resolve generator with RegistrationName {genConfig.Name}");
                }
            }
            return returnVal;
        }
        public List<string> GetFilenames(State state)
        {
            var returnVal = new List<string>();
            foreach (var genConfig in state._3Config.Generators)
            {
                if (_tiny.CanResolve<IGenerator>(genConfig.Name))
                {
                    var generator = _tiny.Resolve<IGenerator>(genConfig.Name);
                    returnVal.Add(generator.GetFilename(state, genConfig.Options));
                }
                else
                {
                    Console.WriteLine($"Cannot resolve generator with RegistrationName {genConfig.Name}");
                }
            }
            return returnVal;
        }

    }
}
