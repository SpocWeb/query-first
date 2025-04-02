using System;
using System.Collections.Generic;


namespace SqlServerTestTarget
{
    using FluentAssertions;
    using Queries;
    using QueryFirst;
    using QueryFirst.IntegrationTests;

    public class Program
    {
        public static void Main(string[] args)
        {
            var testDB = new TestDB();
            var result = new GetOneRowQfRepo(testDB).Execute();
            result.ForEach(l => Console.WriteLine($"{l.Id} {l.MyChar}"));

            // static
            QueryFirstConnectionFactory.Instance = testDB;
            var staticResult = GetOneRowQfRepo.ExecuteStatic();

            var msgRepo = new ReturnInfoMessageQfRepo(testDB);
            var infoMsgResult = msgRepo.ExecuteNonQuery();
            Console.WriteLine(msgRepo.ExecutionMessages);

            // Test Dynamic OrderBy
            var query = new TestDynamicOrderByQfRepo(testDB);
            var sorted = query.Execute(new[] { (TestDynamicOrderByQfRepo.Cols.MyVarchar, true) });

            Console.WriteLine(sorted[0].MyVarchar); // should be Xavier

            var asyncResult = new GetOneRowAsyncQfRepo(testDB).ExecuteScalarAsync().Result;
            Console.WriteLine(asyncResult);

            var carefullySelectedResult = new LoadsaParametersQfRepo(testDB).ExecuteScalar(
                myBigint: 9_223_372_036_854_775_807,
                myBit: true,
                myDecimal: 1234.567m,
                myInt: 1234,
                myMoney: 1234,
                mySmallint: 1234,
                myTinyint: 255,
                myFloat: 123.456,
                myReal: 123.456f,
                myDate: DateTime.Now,
                myDatetime2: DateTime.Now,
                myDatetime: DateTime.Now,
                myChar: "hello cobber",
                myVarchar: "Antoine",
                myNchar: "loadsa κόσμε",
                myNvarchar: "async result κόσμε"
            );
            Console.WriteLine(carefullySelectedResult);
            var expandableIn = new ExpandableInQfRepo(testDB).Execute(new List<int?> { 1234, 1235 });
            Console.WriteLine($"ExpandableIn returns {expandableIn.Count} rows");

            var staticRslt = GetOneRowAsyncQfRepo.ExecuteStatic();

            var dyn = DynamicInIntQfRepo.ExecuteStatic(new List<int?> { 1234, 1235 });
            dyn.Count.Should().Be(2);

            TableValuedParametersQfRepo.ExecuteNonQueryStatic(
                new List<TableValuedParametersQfRepo.TestTableType> {
                    new() {MyTVPVarchar = "cobber", MYTVPInt = 1236},
                    new() {MyTVPVarchar = "cobber", MYTVPInt = 1237}
                }
            );
        }
    }
    public static class QueryfirstDefaultConnection
    {
        public static string ConnectionString;
    }

}
