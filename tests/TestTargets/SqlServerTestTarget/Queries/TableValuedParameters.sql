/* .sql query managed by QueryFirst add-in */
-- designTime - put parameter declarations and design time initialization here
DECLARE @MyTableValuedParam TestTableType;
-- endDesignTime


      INSERT into EveryDatatype (MyVarchar, MyInt) 
      (SELECT * FROM @MyTableValuedParam)