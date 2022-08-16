namespace QueryFirst.IntegrationTests.Queries
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.IO;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Text.RegularExpressions;

	using static DropAndCreateTestDBQfRepo;


	using System.Data.SqlClient;
	using System.Threading.Tasks;

	public interface IDropAndCreateTestDBQfRepo
	{

		List<DropAndCreateTestDBQfDto> Execute();
		IEnumerable<DropAndCreateTestDBQfDto> Execute(IDbConnection conn, IDbTransaction tx = null);
		System.String ExecuteScalar();
		System.String ExecuteScalar(IDbConnection conn, IDbTransaction tx = null);

		DropAndCreateTestDBQfDto Create(IDataRecord record);
		DropAndCreateTestDBQfDto GetOne();
		DropAndCreateTestDBQfDto GetOne(IDbConnection conn, IDbTransaction tx = null);
		int ExecuteNonQuery();
		int ExecuteNonQuery(IDbConnection conn, IDbTransaction tx = null);
	}
	public partial class DropAndCreateTestDBQfRepo : IDropAndCreateTestDBQfRepo
	{

		void AppendExececutionMessage(string msg) { ExecutionMessages += msg + Environment.NewLine; }
		public string ExecutionMessages { get; protected set; }
		// constructor with connection factory injection
		protected QueryFirst.IQfDbConnectionFactory _connectionFactory;
		public DropAndCreateTestDBQfRepo(QueryFirst.IQfDbConnectionFactory connectionFactory)
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
USE [QueryFirstTestDB]
;
/****** Object:  Table [dbo].[EveryDatatype]    Script Date: 16/08/2022 11:49:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EveryDatatype]') AND type in (N'U'))
DROP TABLE [dbo].[EveryDatatype]
;
USE [master]
;
/****** Object:  Database [QueryFirstTestDB]    Script Date: 16/08/2022 11:49:57 ******/
DROP DATABASE [QueryFirstTestDB]
;
/****** Object:  Database [QueryFirstTestDB]    Script Date: 16/08/2022 11:49:57 ******/
CREATE DATABASE [QueryFirstTestDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'QueryFirstTestDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\QueryFirstTestDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'QueryFirstTestDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\QueryFirstTestDB_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
;
ALTER DATABASE [QueryFirstTestDB] SET COMPATIBILITY_LEVEL = 140
;
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [QueryFirstTestDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
;
ALTER DATABASE [QueryFirstTestDB] SET ANSI_NULL_DEFAULT OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET ANSI_NULLS OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET ANSI_PADDING OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET ANSI_WARNINGS OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET ARITHABORT OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET AUTO_CLOSE OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET AUTO_SHRINK OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET AUTO_UPDATE_STATISTICS ON 
;
ALTER DATABASE [QueryFirstTestDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET CURSOR_DEFAULT  GLOBAL 
;
ALTER DATABASE [QueryFirstTestDB] SET CONCAT_NULL_YIELDS_NULL OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET NUMERIC_ROUNDABORT OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET QUOTED_IDENTIFIER OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET RECURSIVE_TRIGGERS OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET  DISABLE_BROKER 
;
ALTER DATABASE [QueryFirstTestDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET TRUSTWORTHY OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET PARAMETERIZATION SIMPLE 
;
ALTER DATABASE [QueryFirstTestDB] SET READ_COMMITTED_SNAPSHOT OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET HONOR_BROKER_PRIORITY OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET RECOVERY FULL 
;
ALTER DATABASE [QueryFirstTestDB] SET  MULTI_USER 
;
ALTER DATABASE [QueryFirstTestDB] SET PAGE_VERIFY CHECKSUM  
;
ALTER DATABASE [QueryFirstTestDB] SET DB_CHAINING OFF 
;
ALTER DATABASE [QueryFirstTestDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
;
ALTER DATABASE [QueryFirstTestDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
;
ALTER DATABASE [QueryFirstTestDB] SET DELAYED_DURABILITY = DISABLED 
;
EXEC sys.sp_db_vardecimal_storage_format N'QueryFirstTestDB', N'ON'
;
ALTER DATABASE [QueryFirstTestDB] SET QUERY_STORE = OFF
;
USE [QueryFirstTestDB]
;
/****** Object:  Table [dbo].[EveryDatatype]    Script Date: 16/08/2022 11:49:57 ******/
SET ANSI_NULLS ON
;
SET QUOTED_IDENTIFIER ON
;
CREATE TABLE [dbo].[EveryDatatype](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MyBigint] [bigint] NULL,
	[MyBit] [bit] NULL,
	[MyDecimal] [decimal](18, 0) NULL,
	[MyInt] [int] NULL,
	[MyMoney] [money] NULL,
	[MyNumeric] [numeric](18, 0) NULL,
	[MySmallint] [smallint] NULL,
	[MyTinyint] [tinyint] NULL,
	[MyFloat] [float] NULL,
	[MyReal] [real] NULL,
	[MyDate] [date] NULL,
	[MyDatetime2] [datetime2](7) NULL,
	[MyDatetime] [datetime] NULL,
	[MyChar] [char](10) NULL,
	[MyVarchar] [varchar](50) NULL,
	[MyText] [text] NULL,
	[MyNchar] [nchar](10) NULL,
	[MyNvarchar] [nvarchar](50) NULL,
	[MyNtext] [ntext] NULL,
	[MyBinary] [binary](50) NULL,
	[MyVarbinary] [varbinary](50) NULL,
	[MyImage] [image] NULL,
 CONSTRAINT [PK_EveryDatatype] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;
USE [master]
;
ALTER DATABASE [QueryFirstTestDB] SET  READ_WRITE 
;

";
			// QfExpandoParams

			return queryText;
		}
		#region Sync

		public virtual List<DropAndCreateTestDBQfDto> Execute()
		{
			using (IDbConnection conn = _connectionFactory.CreateConnection())
			{
				conn.Open();
				var returnVal = Execute(conn).ToList();
				return returnVal;
			}
		}
		public virtual IEnumerable<DropAndCreateTestDBQfDto> Execute(IDbConnection conn, IDbTransaction tx = null)
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
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return Create(reader);
					}
				}

				// Assign output parameters to instance properties. These will be available after this method returns.
				// todo : make output parameters work in a threadsafe way. An output object with execution messages and output parameters?
				/*

				*/
			}
		}

		public virtual DropAndCreateTestDBQfDto GetOne()
		{
			using (IDbConnection conn = _connectionFactory.CreateConnection())
			{
				conn.Open();
				var returnVal = GetOne(conn);
				return returnVal;
			}
		}
		public virtual DropAndCreateTestDBQfDto GetOne(IDbConnection conn, IDbTransaction tx = null)
		{
			// this line will not compile in .net core unless you install the System.Data.SqlClient nuget package.
			((SqlConnection)conn).InfoMessage += new SqlInfoMessageEventHandler(
				delegate (object sender, SqlInfoMessageEventArgs e) { AppendExececutionMessage(e.Message); });
			{
				var all = Execute(conn, tx);
				DropAndCreateTestDBQfDto returnVal;
				using (IEnumerator<DropAndCreateTestDBQfDto> iter = all.GetEnumerator())
				{
					iter.MoveNext();
					returnVal = iter.Current;
				}
				return returnVal;
			}
		}
		public virtual System.String ExecuteScalar()
		{
			using (IDbConnection conn = _connectionFactory.CreateConnection())
			{
				conn.Open();
				var returnVal = ExecuteScalar(conn);
				/*
				;
				*/
				return returnVal;
			}
		}

		public virtual System.String ExecuteScalar(IDbConnection conn, IDbTransaction tx = null)
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
				var result = cmd.ExecuteScalar();

				// only convert dbnull if nullable
				// Assign output parameters to instance properties.
				/*

				*/
				if (result == null || result == DBNull.Value)
					return null;
				else
					return (System.String)result;
			}
		}


		#endregion

		public virtual DropAndCreateTestDBQfDto Create(IDataRecord record)
		{
			var returnVal = CreatePoco(record);

			if (record[0] != null && record[0] != DBNull.Value)
				returnVal.Database Name = (string)record[0];

			if (record[1] != null && record[1] != DBNull.Value)
				returnVal.Vardecimal State = (string)record[1];

			// provide a hook to override
			returnVal.OnLoad();
			return returnVal;
		}
		protected virtual DropAndCreateTestDBQfDto CreatePoco(System.Data.IDataRecord record)
		{
			return new DropAndCreateTestDBQfDto();
		}
		protected void AddParameters(IDbCommand cmd)
		{
		}
	}
	public partial class DropAndCreateTestDBQfDto
	{
		protected string _Database Name; // nvarchar(128) not null
public string Database Name{
get{return _Database Name;
	}
	set{_Database Name = value;
}
}
protected string _Vardecimal State; // varchar(3) not null
public string Vardecimal State
{
	get{ return _Vardecimal State; }
	set{ _Vardecimal State = value; }
}
protected internal virtual void OnLoad() { }
}
}
