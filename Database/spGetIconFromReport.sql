SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spGetIconFromReport @formdata xml, @iconReturn nvarchar(255) output
AS
BEGIN

	Declare @id int, @xquery nvarchar(max), @type nvarchar(max), @operator nvarchar(2), @compareValue nvarchar(max), @icon nvarchar(255);
	Declare @params nvarchar(255) = '@formdata xml, @outcount int output';
	DECLARE @blob_eater SQL_VARIANT;
	
	-- Load IconSettings into temp table in memory
	Declare @Enumerator table (ID int, Priority int, XQuery varchar(max), SQLType varchar(max), Operator nvarchar(2), CompareValue nvarchar(max), Icon nvarchar(255))
	Insert into @Enumerator Select ID, Priority, XQuery, SqlType, Operator, CompareValue, Icon from IconSettings
	
	-- Loop through each item to see if it returns positive.  
	-- If so, get icon from that record, break loop, and return the icon
	-- If not, remove that record from the temp table and continue
	While exists (select 1 from @Enumerator)
	Begin
		Select top 1 @id = ID, @xquery = XQuery, @type = SQLType, @operator = Operator, @compareValue = CompareValue from @Enumerator order by priority desc;

		Declare @sql nvarchar(max) = 'Select @blob_eater = 1 where @formdata.value(''' + @xquery + ''', ''' + @type + ''') ' + @operator + '''' + @compareValue + '''';
		Declare @outcount int;
		exec sp_executesql @sql, @params, @formdata = @formdata, @outcount = @outcount output;

		if @outcount > 0
		begin
			Select @iconReturn = Icon from IconSettings where ID = @id;
			break;
		end
		
		Delete from @Enumerator where @id = id;
	End
	Delete from @Enumerator;

	Select @iconReturn = 'default.png';
END
GO