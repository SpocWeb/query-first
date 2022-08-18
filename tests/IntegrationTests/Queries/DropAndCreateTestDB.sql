-- queryfirst
-- not working. hangs. what the hell
WHILE EXISTS(select NULL from sys.databases where name='QueryFirstTestDB')
BEGIN
    DECLARE @SQL varchar(max)
    SELECT @SQL = COALESCE(@SQL,'') + 'Kill ' + Convert(varchar, SPId) + ';'
    FROM MASTER..SysProcesses
    WHERE DBId = DB_ID(N'QueryFirstTestDB') AND SPId <> @@SPId
	PRINT @SQL
    EXEC(@SQL)
	WITH RESULT SETS NONE
    DROP DATABASE [QueryFirstTestDB]
END
;
--sp_who2

CREATE DATABASE QueryFirstTestDB
;

USE QueryFirstTestDB
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
	[MyChar] [char](255) NULL,
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
RETURN

select * from everydatatype

SELECT text, GETDATE(), *
FROM sys.dm_exec_requests
CROSS APPLY sys.dm_exec_sql_text(sql_handle)

sp_who2