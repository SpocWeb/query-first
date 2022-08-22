/* .sql query managed by QueryFirst add-in */
-- designTime
declare @MyBigint bigint;
declare @MyBit bit;
declare @MyDecimal decimal(18,9);
declare @MyInt int;
declare @MyMoney money;
declare @MySmallint smallint;
declare @MyTinyint tinyint;
declare @MyFloat float;
declare @MyReal real;
declare @MyDate date;
declare @MyDatetime2 datetime2(7);
declare @MyDatetime datetime;
declare @MyChar char(255);
declare @MyVarchar varchar(50);
declare @MyNchar nchar(255);
declare @MyNvarchar nvarchar(50);
declare @MyBinary binary(50);
declare @MyVarbinary varbinary(50);

-- endDesignTime

INSERT INTO EveryDatatype (
MyBigint,
MyBit,
MyDecimal,
MyInt,
MyMoney,
MySmallint,
MyTinyint,
MyFloat,
MyReal,
MyDate,
MyDatetime2,
MyDatetime,
MyChar,
MyVarchar,
MyNchar,
MyNvarchar,
MyBinary,
MyVarbinary
)
VALUES (
@MyBigint,
@MyBit,
@MyDecimal,
@MyInt,
@MyMoney,
@MySmallint,
@MyTinyint,
@MyFloat,
@MyReal,
@MyDate,
@MyDatetime2,
@MyDatetime,
@MyChar,
@MyVarchar,
@MyNchar,
@MyNvarchar,
@MyBinary,
@MyVarbinary
)

SELECT CAST(SCOPE_IDENTITY() AS INT) AS JustInsertedId
