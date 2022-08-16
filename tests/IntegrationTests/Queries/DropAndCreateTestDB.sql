/* .sql query managed by QueryFirst add-in */
-- designTime
-- endDesignTime
USE [QueryFirstTestDB]
GO
/****** Object:  Table [dbo].[EveryDatatype]    Script Date: 16/08/2022 11:49:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EveryDatatype]') AND type in (N'U'))
DROP TABLE [dbo].[EveryDatatype]
GO
USE [master]
GO
/****** Object:  Database [QueryFirstTestDB]    Script Date: 16/08/2022 11:49:57 ******/
DROP DATABASE [QueryFirstTestDB]
GO
/****** Object:  Database [QueryFirstTestDB]    Script Date: 16/08/2022 11:49:57 ******/
CREATE DATABASE [QueryFirstTestDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'QueryFirstTestDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\QueryFirstTestDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'QueryFirstTestDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\QueryFirstTestDB_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [QueryFirstTestDB] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [QueryFirstTestDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [QueryFirstTestDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [QueryFirstTestDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [QueryFirstTestDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [QueryFirstTestDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [QueryFirstTestDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET RECOVERY FULL 
GO
ALTER DATABASE [QueryFirstTestDB] SET  MULTI_USER 
GO
ALTER DATABASE [QueryFirstTestDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [QueryFirstTestDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [QueryFirstTestDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [QueryFirstTestDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [QueryFirstTestDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'QueryFirstTestDB', N'ON'
GO
ALTER DATABASE [QueryFirstTestDB] SET QUERY_STORE = OFF
GO
USE [QueryFirstTestDB]
GO
/****** Object:  Table [dbo].[EveryDatatype]    Script Date: 16/08/2022 11:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
USE [master]
GO
ALTER DATABASE [QueryFirstTestDB] SET  READ_WRITE 
GO

