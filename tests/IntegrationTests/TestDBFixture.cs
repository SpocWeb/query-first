﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QueryFirst.IntegrationTests
{
    using Microsoft.Extensions.Configuration;
    using Queries;
    using Xunit;

    public class TestDBFixture : IDisposable
    {
        public TestDBFixture()
        {
    //        var configuration = new ConfigurationBuilder()
    //.AddInMemoryCollection(appSettingsStub)
    //.Build();
            var testDB = new QfDbConnectionFactory();
            var master = new MasterConnectionFactory();

            //new DropAndCreateTestDBQfRepo(master).ExecuteNonQuery();
            new DeleteAllQfRepo(testDB).ExecuteNonQuery();

            var insert = new InsertQfRepo(testDB);
            insert.ExecuteNonQuery(
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
                myNchar: "κόσμε",
                myNvarchar: "κόσμε",
                myBinary: new byte[] {
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20
                },
                myVarbinary: new byte[] {
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20
                }
            );
            insert.ExecuteNonQuery(
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
                myVarchar: "Xavier",
                myNchar: "κόσμε",
                myNvarchar: "κόσμε",
                myBinary: new byte[] {
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20
                },
                myVarbinary: new byte[] {
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,
                    0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20
                }
            );
        }
        public void Dispose() { }
    }
    [CollectionDefinition("QfTestCollection")]
    public class QfTestCollection : ICollectionFixture<TestDBFixture> { }
}
