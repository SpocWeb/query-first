using EnvDTE;
using Microsoft.VisualStudio.Shell;
using QueryFirst.Generators;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using TinyIoC;

//[assembly: InternalsVisibleTo("QueryFirstTests")]


namespace QueryFirst.VSExtension
{
    public class VsixConductor
    {
        private State _state;
        private TinyIoCContainer _tiny;
        private VSOutputWindow _vsOutputWindow;
        private Document _queryDoc;
        private ProjectItem _item;
        private IProvider _provider;
        private WrapperClassMaker _wrapper;
        private IResultClassMaker _results;
        private static readonly string n = Environment.NewLine;

        public VsixConductor(VSOutputWindow vSOutputWindow, WrapperClassMaker wrapperClassMaker, IResultClassMaker resultClassMaker)
        {
            _tiny = TinyIoCContainer.Current;
            _wrapper = wrapperClassMaker ?? _tiny.Resolve<WrapperClassMaker>();
            _results = resultClassMaker ?? _tiny.Resolve<IResultClassMaker>();

            _vsOutputWindow = vSOutputWindow;
        }

        public State ProcessOneQuery(Document queryDoc, bool headless = false)
        {
            _state = new State();
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                _queryDoc = queryDoc;
                _item = queryDoc.ProjectItem;

                // same drill as command line.
                // fetch config query/project/install
                var configFileReader = new ConfigFileReader();
                var projectConfig = configFileReader.GetProjectConfig(_queryDoc.FullName);
                var installConfig = configFileReader.GetInstallConfig();

                var projectType = new ProjectType().DetectProjectType();

                // build config project-install
                var configBuilder = new ConfigBuilder();
                var outerConfig = configBuilder.Resolve2Configs(configBuilder.GetInstallConfigForProjectType(installConfig, projectType), projectConfig);

                // register types
                RegisterTypes.Register(outerConfig.HelperAssemblies);



                ProcessUpToStep4(queryDoc.FullName, outerConfig, ref _state);

                // Test this! If I can get source control exclusions working, team members won't get the generated file.
                //if (!File.Exists(_state._1QfRepoFullFilename))
                //{
                //    var _ = File.Create(_state._1QfRepoFullFilename);
                //    _.Dispose();
                //}
                //if (GetItemByFilename(queryDoc.ProjectItem, _state._1QfRepoFullFilename) != null)
                //    queryDoc.ProjectItem.Collection.AddFromFile(_state._1QfRepoFullFilename);

                // We have the config, we can instantiate our provider...
                if (_tiny.CanResolve<IProvider>(_state._3Config.Provider))
                    _provider = _tiny.Resolve<IProvider>(_state._3Config.Provider);
                else
                    _vsOutputWindow.Write(@"After resolving the config, we have no provider\n");



                if (string.IsNullOrEmpty(_state._3Config.DefaultConnection))
                {
                    _vsOutputWindow.Write(@"No design time connection string. You need to create qfconfig.json beside or above your query 
or put --QfDefaultConnection=myConnectionString somewhere in your query file.
See the Readme section at https://marketplace.visualstudio.com/items?itemName=bbsimonbb.QueryFirst.VSExtension    
");
                    return _state; // nothing to be done

                }
                if (!_tiny.CanResolve<IProvider>(_state._3Config.Provider))
                {
                    _vsOutputWindow.Write(string.Format(
@"No Implementation of IProvider for providerName {0}. 
The query {1} may not run and the wrapper has not been regenerated.\n",
                    _state._3Config.Provider, _state._1BaseName
                    ));
                    return _state;
                }
                // Scaffold inserts and updates
                _tiny.Resolve<_5ScaffoldUpdateOrInsert>().Go(ref _state);

                if (_state._2InitialQueryText != _state._5QueryAfterScaffolding)
                {
                    var textDoc = ((TextDocument)queryDoc.Object());
                    var ep = textDoc.CreateEditPoint();
                    ep.ReplaceText(_state._2InitialQueryText.Length, _state._5QueryAfterScaffolding, 0);
                }


                // Execute query
                try
                {
                    new _6FindUndeclaredParameters(_provider).Go(ref _state, out string outputMessage);
                    // if message returned, write it to output.
                    if (!string.IsNullOrEmpty(outputMessage))
                        _vsOutputWindow.Write(outputMessage);
                    // if undeclared params were found, add them to the .sql
                    // only do this when editing the sql. If we're regenerating all, skip this step.
                    if (!string.IsNullOrEmpty(_state._6NewParamDeclarations) && !headless)
                    {
                        ReplacePattern("-- endDesignTime", _state._6NewParamDeclarations + "-- endDesignTime");
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
                // ---------------------- Generate code ----------------------------------------------------------------------------
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
                                _vsOutputWindow.Write($"QueryFirst DELETED {codeFile.Filename + Environment.NewLine}");
                            }
                        }
                        else
                        {
                            var genFile = GetItemByFilename(queryDoc.ProjectItem, codeFile.Filename);
                            // target in the project. We'll edit it in Visual Studio, with formatting.
                            if (genFile != null)
                                WriteAndFormat(genFile, codeFile.FileContents);
                            // target not in the project. We'll just write it out like a normal file.
                            else
                                fileWriter.WriteFile(codeFile);
                            _vsOutputWindow.Write($"QueryFirst wrote {codeFile.Filename + Environment.NewLine}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                _vsOutputWindow.Write($"ERROR processing {queryDoc.FullName}{n}{ex.TellMeEverything()}{n}");
            }
            return _state;
        }
        private void WriteAndFormat(ProjectItem genFile, string code)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            bool rememberToClose = false;
            if (!genFile.IsOpen)
            {
                genFile.Open();
                rememberToClose = true;
            }
            var textDoc = ((TextDocument)genFile.Document.Object());
            var ep = textDoc.CreateEditPoint();
            ep.ReplaceText(textDoc.EndPoint, code, 0);
            ep.SmartFormat(textDoc.EndPoint);
            genFile.Save();
            if (rememberToClose)
            {
                genFile.Document.Close();
            }
        }
        /// <summary>
        /// Now we can connect the editor window, we need to recover the connection string when we open a query.
        /// This method is called on open and on save.
        /// </summary>
        /// <param name="queryDoc"></param>
        /// <param name="state"></param>
        internal void ProcessUpToStep4(string sourcePath, QfConfigModel outerConfig, ref State state)
        {
            // todo: if a .sql is not in the project, this throws null exception. What should it do?
            new _1ProcessQueryPath().Go(state, sourcePath);


            new _2ReadQuery().Go(state);
            new _3ResolveConfig().BuildUp().Go(state, outerConfig);
            new _4ResolveNamespace().Go(state);
        }
        // Doesn't recurse into folders. Prefer items.Item("")
        public static ProjectItem GetItemByFilename(ProjectItem item, string filename)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (item == null)
                throw new ArgumentException(nameof(item));
            if (filename == null)
                throw new ArgumentException(nameof(filename));

            foreach (ProjectItem childItem in item.ProjectItems)
            {
                for (short i = 0; i < childItem.FileCount; i++)
                {
                    if (childItem.FileNames[i].Equals(filename))
                        return childItem as ProjectItem;
                }
            }

            // .net core has a little problem with nested items.
            foreach (ProjectItem childItem in item.Collection)
            {
                if (childItem.FileNames[0].ToLower() == filename.ToLower())
                {
                    return childItem as ProjectItem;
                }
            }
            return null;
        }
        private void ReplacePattern(string pattern, string replaceWith)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var textDoc = ((TextDocument)_queryDoc.Object());
            textDoc.ReplacePattern(pattern, replaceWith, 0, null);
        }
    }
}

