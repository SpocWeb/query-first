/* .sql query managed by QueryFirst add-in */
-- designTime - put parameter declarations and design time initialization here
declare @ListOfIds int;
-- endDesignTime
SELECT * FROM EveryDatatype E WHERE E.MyInt IN(@ListOfIds)