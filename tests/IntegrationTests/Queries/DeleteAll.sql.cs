﻿namespace QueryFirst.IntegrationTests.Queries
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text.RegularExpressions;

    using static DeleteAllQfRepo;


    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public interface IDeleteAllQfRepo
    {

        int ExecuteNonQuery();
        int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
    }
    public partial class DeleteAllQfRepo : IDeleteAllQfRepo
    {

        void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
        public string ExecutionMessages { get; protected set; }
        // constructor with connection factory injection
        protected QueryFirst.IQfDbConnectionFactory _connectionFactory;
        public DeleteAllQfRepo(QueryFirst.IQfDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #region Sync
        public virtual int ExecuteNonQuery()
        {
            using (IDbConnection conn = _connectionFactory.CreateConnection())
            {
                conn.Open();
                return ExecuteNonQuery(conn);
            }
        }
        public virtual int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null)
        {

            // this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
            ((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
                delegate (object sender, SqlInfoMessageEventArgs e) { AppendExececutionMessage(e.Message); });
            using (IDbCommand cmd = conn.CreateCommand())
            {
                if (tx != null)
                    cmd.Transaction = tx;
                cmd.CommandText = getCommandText();
                AddParameters(cmd);
                var result = cmd.ExecuteNonQuery();

                // Assign output parameters to instance properties. 
                /*

                */
                // only convert dbnull if nullable
                return result;
            }
        }


        #endregion

        public string getCommandText()
        {
            var queryText = $@"/* .sql query managed by QueryFirst add-in */
/*designTime

endDesignTime*/
DELETE from EveryDatatype;

";
            // QfExpandoParams

            return queryText;
        }
        protected void AddParameters(IDbCommand cmd)
        {
        }
    }
}
