/* .sql query managed by QueryFirst add-in */
-- designTime - put parameter declarations and design time initialization here
declare @MyBigint bigint;
declare @MyBit bit;
declare @MyDecimal decimal(18,0);
declare @MyInt int;
declare @MyMoney money;
declare @MySmallint smallint;
declare @MyTinyint tinyint;
declare @MyFloat float;
declare @MyReal real;
declare @MyDate date;
declare @MyDatetime datetime;
declare @MyDatetime2 datetime2(7);
declare @MyChar char(255);
declare @MyVarchar varchar(50);
declare @MyNchar nchar(255);
declare @MyNvarchar nvarchar(50);
-- endDesignTime
SELECT E.MyNchar FROM EveryDatatype E
WHERE 1=1
and MyBigint = @MyBigint
and MyBit = @MyBit
and MyDecimal = @MyDecimal
and MyInt = @MyInt
and MyMoney = @MyMoney
and MySmallint = @MySmallint
and MyTinyint = @MyTinyint
and MyFloat = @MyFloat
and MyReal = @MyReal
and MyDate <= @MyDate
and MyDatetime <= @MyDatetime
and MyDatetime2 <= @MyDatetime2
and MyChar = @MyChar
and MyVarchar = @MyVarchar
and MyNchar = @MyNchar
and MyNvarchar = @MyNvarchar
