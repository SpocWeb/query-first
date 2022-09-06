# Extending QueryFirst

You can extend QueryFirst, providing your own implementations of nearly everything in the application. Your implementations should be provided in helper assemblies, that you can specify in your project's qfconfig.json file. Implementations in helper assemblies will have priority over the bundled implementations. IProvider and IGenerator implementations should have a QueryFirst.RegistrationName attribute. This lets them live happily beside the bundled implementations. You define a RegistrationName in the code that you can then reference in your configuration file.

## IProvider

This interface contains everything specific to the database you're using. In theory, you should be able to easily implement another database, Oracle for example, by writing an implementation of IProvider.

An example implementation of a provider:

```csharp
// You will need a COM reference to QueryFirst.dll
using QueryFirst;

namespace MyHelperAssembly
{
    [RegistrationName("MyGroovyProvider")]
    public class TestProvider : Providers.MicrosoftDataSqlClient, IProvider
    {
        public override string HookUpForExecutionMessages()
        {
            return base.HookUpForExecutionMessages() + "// hello from MyGroovyProvider";
        }
    }
}
```

then to use this provider, in your project qfconfig.json, or your query's .sql.json...

```json
// qfconfig.json or [MyQuery].sql.json
{
  ...
  "helperAssemblies": [ "C:\\absolute\\path\\to\\TestHelperDll.dll" ], // at the project level
  "provider": "MyGroovyProvider", // on the project, or directly on the query.
  ...
}
```

If you specify your provider at the query level, for performance reasons you should still register the helper assembly in the project qfconfig.json. This will save us having to drop and re-register everything each time we regenerate the query.

## IGenerator

IGenerator Implementations are called with a QueryFirst.State object, containing everything we know about a query, and a string dictionary of options. They return a code file (a string and a filename relative to the project root). You could write an implementation for Visual Basic or Java. QueryFirst currently ships with implementations for C# and Typescript. A given query is associated with one provider, but can have many generators. The options dictionary comes directly from the options section of the generator's config. All generator options must be strings in the config files, and the json schema will not help you discover them. Generators are loosely coupled, and this is the  best we can do.

```csharp
public interface IGenerator
{
    QfTextFile Generate(State state, Dictionary<string, string> options);

    string GetFilename(State state, Dictionary<string, string> options);
}
```

```json
  ...
  "generators": [
    {
      "name": "CSharp",
      "options": { "generateAsync": "true" }
    }
  ]
  ...
```