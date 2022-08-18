/* .sql query managed by QueryFirst add-in */

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