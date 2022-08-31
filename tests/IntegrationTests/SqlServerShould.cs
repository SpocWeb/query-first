using FluentAssertions;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace IntegrationTests
{
    [Collection("SqlServerTestCollection")]
    public class SqlServerShould
    {
        [Fact]
        public void RegenerateAll_ShouldRegenerateAllAndBuildAndRun()
        {
            // Clean
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var projectToRegenerate = Path.Combine(assemblyPath, @"../../../../TestTargets/SqlServerTestTarget/");

            Directory.GetFiles(projectToRegenerate, "*.sql.cs", SearchOption.AllDirectories).ToList().ForEach(f => File.Delete(f));
            Directory.Delete(Path.Combine(projectToRegenerate, "bin"), true);
            // Regenerate all
            var queryFirstDebugBuild = Path.Combine(assemblyPath, @"../../../../../QueryFirst.CommandLine/bin/Debug/net6.0/QueryFirst.exe");
            var result = RunProcess(queryFirstDebugBuild, projectToRegenerate);

            //Assert.Matches(@"^Processed file.*GetOneRow\.sql'\.$", result.stdOut);
            result.stdOut.Should().Contain("GetOneRow.sql");
            result.stdErr.Should().BeEmpty();

            // Dependency Injection. Groovy provider should be used for async
            var asyncCodeFile = File.ReadAllText(Path.Combine(assemblyPath, @"../../../../TestTargets/SqlServerTestTarget/Queries/GetOneRowAsync.sql.cs"));
            asyncCodeFile.Should().Contain("hello from MyGroovyProvider");

            // Build it
            var buildResult = RunProcess("dotnet", $"build {projectToRegenerate} -c Debug");
            buildResult.stdErr.Should().BeEmpty();
            buildResult.stdOut.Contains("ÉCHEC").Should().BeFalse("the project should build.");
            buildResult.stdOut.Contains("FAILED").Should().BeFalse("the project should build.");

            // Run it
            var runResult = RunProcess(Path.Combine(assemblyPath, @"../../../../TestTargets/SqlServerTestTarget/bin/Debug/net5.0/SqlServerTestTarget.exe"));
            runResult.stdErr.Should().BeEmpty();
            runResult.stdOut.Should().Contain("hello cobber", "GetOneRow should contain 'hello cobber'.");
            runResult.stdOut.Should().Contain("info message", "ReturnInfoMessage should return this text");
            runResult.stdOut.Should().Contain("Xavier", "TestDynamicOrderBy should return Xavier, not Antoine.");
            runResult.stdOut.Should().Contain("async result", "GetOneRowAsync should return 'async result'");
            runResult.stdOut.Should().Contain("loadsa", "LoadsaParameters should return 'loadsa'");
            runResult.stdOut.Should().Contain("ExpandableIn returns 2 rows");
            

        }
        private (string stdOut, string stdErr) RunProcess(string filename, string args = "")
        {
            var psi = new ProcessStartInfo(filename, args)
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            // force english for MSBUILD (not working)
            if (psi.EnvironmentVariables.ContainsKey("VSLANG"))
                psi.EnvironmentVariables["VSLANG"] = "1033";
            else
                psi.EnvironmentVariables.Add("VSLANG", "1033");

            // force english for dotnet
            if (psi.EnvironmentVariables.ContainsKey("DOTNET_CLI_UI_LANGUAGE"))
                psi.EnvironmentVariables["DOTNET_CLI_UI_LANGUAGE"] = "en";
            else
                psi.EnvironmentVariables.Add("DOTNET_CLI_UI_LANGUAGE", "en");
            // necessary when using environment variables
            psi.UseShellExecute = false;
            using var process = Process.Start(psi);
            var stdOut = process.StandardOutput.ReadToEnd();
            var stdErr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return (stdOut, stdErr);
        }
    }
}
