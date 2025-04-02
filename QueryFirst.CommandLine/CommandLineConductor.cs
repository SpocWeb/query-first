using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

//[assembly: InternalsVisibleTo("QueryFirstTests")]


namespace QueryFirst.CommandLine
{
    // writes to console, uses tiny, doesn't interact with file system,
    public class CommandLineConductor
    {
        private State _state;
        private TinyIoCContainer _tiny;
        private IProvider _provider;

        public CommandLineConductor()
        {
            _tiny = TinyIoCContainer.Current;
        }

        // DI via property injection
        public IQfTextFileWriter QfTextFileWriter { get; set; }

        public void ProcessOneQuery(string sourcePath, QfConfigModel outerConfig)
        {
            try
            {
                _state = new State();

                ProcessUpToStep4(sourcePath, outerConfig, ref _state);

                // We have the config, we can instantiate our provider...
                if (_tiny.CanResolve<IProvider>(_state._3Config.Provider))
                    _provider = _tiny.Resolve<IProvider>(_state._3Config.Provider);
                else
                    QfConsole.WriteLine(@"After resolving the config, we have no provider\n");



                if (string.IsNullOrEmpty(_state._3Config.DefaultConnection))
                {
                    QfConsole.WriteLine(@"No design time connection string. You need to create qfconfig.json beside or above your query 
or put --QfDefaultConnection=myConnectionString somewhere in your query file.
See the Readme section at https://marketplace.visualstudio.com/items?itemName=bbsimonbb.QueryFirst.CommandLine    
");
                    return; // nothing to be done

                }
                if (!_tiny.CanResolve<IProvider>(_state._3Config.Provider))
                {
                    QfConsole.WriteLine(string.Format(
@"No Implementation of IProvider for providerName {0}. 
The query {1} may not run and the wrapper has not been regenerated.\n",
                    _state._3Config.Provider, _state._1BaseName
                    ));
                    return;
                }
                // assign some names
                _4ResolveNamespace.Go(_state);

                // Scaffold inserts and updates
                _tiny.Resolve<_5ScaffoldUpdateOrInsert>().Go(ref _state);

                if (_state._2InitialQueryText != _state._5QueryAfterScaffolding)
                {

                }


                // Execute query
                try
                {
                    new _6FindUndeclaredParameters(_provider).Go(ref _state, out string outputMessage);
                    // if message returned, write it to output.
                    if (!string.IsNullOrEmpty(outputMessage))
                        QfConsole.WriteLine(outputMessage);
                    // if undeclared params were found, modify the original .sql
                    if (!string.IsNullOrEmpty(_state._6NewParamDeclarations))
                    {
                        QfTextFileWriter.WriteFile(new QfTextFile()
                        {
                            Filename = _state._1SourceQueryFullPath,
                            FileContents = _state._6QueryWithParamsAdded
                        });
                    }

                    new _7RunQueryAndGetResultSchema(new AdoSchemaFetcher(), _provider).Go(ref _state);
                    new _8ParseOrFindDeclaredParams(_provider).Go(ref _state);
                }
                catch (Exception ex)
                {
                    var generatedFiles = new InstantiateAndCallGenerators().GetFilenames(_state);
                    foreach (var filename in generatedFiles)
                    {
                        StringBuilder bldr = new StringBuilder();
                        bldr.AppendLine("Error running query.");
                        bldr.AppendLine();
                        bldr.AppendLine("/*The last attempt to run this query failed with the following error. This class is no longer synced with the query");
                        bldr.AppendLine("You can compile the class by deleting this error information, but it will likely generate runtime errors.");
                        bldr.AppendLine("-----------------------------------------------------------");
                        bldr.AppendLine(ex.Message);
                        bldr.AppendLine("-----------------------------------------------------------");
                        bldr.AppendLine(ex.StackTrace);
                        bldr.AppendLine("*/");
                        File.AppendAllText(filename, bldr.ToString());
                    }
                    throw;
                }

                // dump state for reproducing issues
#if DEBUG
                using (var ms = new MemoryStream())
                {
                    var ser = new DataContractJsonSerializer(typeof(State));
                    ser.WriteObject(ms, _state);
                    byte[] json = ms.ToArray();
                    ms.Close();
                    File.WriteAllText(_state._1CurrDir + "qfDumpState.json", Encoding.UTF8.GetString(json, 0, json.Length));
                }
#endif
                var codeFiles = new InstantiateAndCallGenerators().Go(_state);
                var fileWriter = new QfTextFileWriter();
                foreach (var codeFile in codeFiles)
                {
                    if (codeFile != null)
                    {
                        if (codeFile.DeleteMe)
                        {
                            if (File.Exists(codeFile.Filename))
                            {
                                File.Delete(codeFile.Filename);
                                QfConsole.WriteLine($"QueryFirst.CommandLine DELETED {codeFile.Filename + Environment.NewLine}");
                            }
                        }
                        else
                        {
                            fileWriter.WriteFile(codeFile);
                            QfConsole.WriteLine($"QueryFirst.CommandLine wrote {codeFile.Filename + Environment.NewLine}");
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                QfConsole.WriteLine(ex.TellMeEverything());
            }
        }



        /// <summary>
        /// Now we can connect the editor window, we need to recover the connection string when we open a query.
        /// This method is called on open and on save.
        /// </summary>
        internal static void ProcessUpToStep4(string sourcePath, QfConfigModel outerConfig, ref State state)
        {
            // todo: if a .sql is not in the project, this throws null exception. What should it do?
            _1ProcessQueryPath.Go(state, sourcePath);


            _2ReadQuery.Go(state);
            var _3 = new _3ResolveConfig().BuildUp();
            _3.Go(state, outerConfig);
        }


    }
    public class ProcessOneQueryResult
    {
        public string ModifiedSql { get; set; }
        public List<QfTextFile> GeneratedFiles { get; set; }
    }
}

