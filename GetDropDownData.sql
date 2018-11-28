CREATE PROCEDURE [dbo].[GetDropDownData]
    @TableName NVARCHAR(100) 
AS
BEGIN

DECLARE @sqlCommand varchar(1000)
SET @columnList = @TableName+'PID,Name'

SET @sqlCommand = 'SELECT ' + @columnList + ' FROM '+@TableName

EXEC (@sqlCommand)


END
