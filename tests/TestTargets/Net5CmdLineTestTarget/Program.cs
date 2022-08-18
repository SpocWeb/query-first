using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Net5CmdLineTestTarget
{
    using Queries;
    using QueryFirst.IntegrationTests;

    public class Program
    {
        public static void Main(string[] args)
        {
            var testDB = new QfDbConnectionFactory();
            var result = new GetOneRowQfRepo(testDB).Execute();
            result.ForEach(l => Console.WriteLine($"{l.Id} {l.MyChar}"));

            var msgRepo = new ReturnInfoMessageQfRepo(new QfDbConnectionFactory());
            var infoMsgResult = msgRepo.ExecuteNonQuery();
            Console.WriteLine(msgRepo.ExecutionMessages);

            // Test Dynamic OrderBy
            var query = new TestDynamicOrderByQfRepo(testDB);
            var sorted = query.Execute(new[] { (TestDynamicOrderByQfRepo.Cols.MyVarchar, true) });
            Console.WriteLine(sorted[0].MyVarchar); // should be Xavier

            //var asyncResult = new GetCustomersAsync().ExecuteAsync().Result;

        }
    }

}
