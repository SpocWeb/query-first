using System;
using System.Collections.Generic;

namespace QueryFirst
{
    public interface IGenerator
    {
        /// <summary>
        /// After all the discovery, this is where a generator gets a bundle of state
        /// and generates some code.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="options">A generator will get the string contents of the options property in the config file.</param>
        /// <returns>A string of file contents and a filename. The conductors take charge of
        /// actually writing the file</returns>
        QfTextFile Generate(State state, Dictionary<string, string> options);

        /// <summary>
        /// If a query doesn't run, all generated files should break. Implementations just have
        /// responsibility for declaring their output file. The conductors will take charge
        /// of appending the error.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        string GetFilename(State state, Dictionary<string, string> options);
    }
}