# QueryFirst SelfTest()

## Why?

As you deploy your code to different environments, keeping your database schemas synchronized with your evolving application code is a challenge. It frequently depends on developer rigour, and even automated solutions like Entity Framework migrations and Visual Studio database projects are not totally reliable. QueryFirst doesn't tell you how to do this, but it can help you ensure that it has been done correctly, and that the database on top of which you are deploying is compatible with the code being deployed.

## How?

QueryFirst does this by inserting a self-test method in the generated code. The test runs the query (in schema_only mode, just as we do at design time) against the environment's database (using QueryFirstConnectionFactory) and compares the resulting schema to the schema returned at design time. If the query doesn't run, or if there are breaking changes in the returned schema, the test will fail. (Additional columns at the end of a result set are not a breaking change.) As such, you can point the xunit command line runner at your application binary in its environment (config files resolved, database accessible) and, if everything passes, you have a strong guarantee that your database is available and correctly configured, your queries are all valid and their result sets are the expected ones.

This feature is turned off by default. To turn it on, add this to your config file...

```json
  "makeSelfTest":true,
```

