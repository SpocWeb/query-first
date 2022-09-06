# Getting Started

## Installation

QueryFirst is primarily a Visual Studio extension. Changing requirements for extensions mean that we currently maintain separate versions for VS 2019 and VS2022. You 
can install directly inside Visual Studio (Extensions => Manage Extensions, then search for QueryFirst) or you can download from the marketplace, taking the time to browse the QA and reviews.

* [VS 2019 on the VS marketplace](https://marketplace.visualstudio.com/items?itemName=bbsimonbb.QueryFirst)
* [QueryFirst for VS 2022](https://marketplace.visualstudio.com/items?itemName=bbsimonbb.QueryFirstVS17)

To Use QueryFirst in a project you will need to create a qfconfig.json file and implement the QueryFirstConnectionFactory abstract class.

## qfconfig.json, at the root of your project
This is where you tell QueryFirst what database to use for test-running your query and generating the C# wrapper.

```json
{
  "defaultConnection": "Data Source=localhost;Initial Catalog=Northwind;Integrated Security=True"
} 
```

## QueryFirstConnectionFactory
QueryFirst repositories will use this class to get their runtime database connections. You can copy this class into your project, or you can import it via the QueryFirst.Runtime nuget. You then need to subclass it and implement the `CreateConnection()` method. If you instantiate your QfRepos, supply your subclassed QueryFirstConnectionFactory to the constructor, or register it as a service and let DI take care of it. If you plan to use the static methods on your repositories, you need to assign the Instance property.

```csharp
using System.Data;
using System.Data.SqlClient;

namespace QueryFirst
{
    public abstract class QueryFirstConnectionFactory
    {
        public abstract IDbConnection CreateConnection();
        public static QueryFirstConnectionFactory Instance { get; set; }
    }
}

namespace MySuperProject
{
    public class MyDB : QueryFirst.QueryFirstConnectionFactory
    {
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection("Data Source=my_db_server;Database=Northwind;Trusted_Connection=True;");
        }
    }
}

// then, to use the static methods, in your startup code you will need to instantiate and assign...
QueryFirst.QueryFirstConnectionFactory.Instance = new MyDB();
```

## Create a query
QueryFirst queries live in .sql files, created with the QueryFirst template that you should now see under Add => New items => Visual C#.

Once in the template, your query goes after the `-- endDesignTime` flag.

![QueryFirst template](https://user-images.githubusercontent.com/11962703/75419461-f7bd6e80-5989-11ea-93c9-583105e261d3.png)

If this is too slow for you, and you're in a .net core project (.net 5 or greater), you can just create a .sql file and put the text "queryfirst" (case insensitive) somewhere on the first line. Use the [Add New File](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.AddNewFile) extension until its functionality [finally shows up in visual studio](https://twitter.com/mkristensen/status/1561823791553998848).
