/* .sql query managed by QueryFirst add-in */
-- designTime
declare @ListOfInts int;

-- endDesignTime
SELECT * from EveryDatatype E
WHERE E.MyInt in (@ListOfInts)

