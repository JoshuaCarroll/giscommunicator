SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION PercentToHexString
(
	@percentage decimal(5,2)
)
RETURNS char(2)
AS
BEGIN
	DECLARE @Hex varbinary(32) = CONVERT(varbinary, @percentage * 255 ), @HexString char(2) = NULL 
	SELECT @HexString = Right(CONVERT(VARCHAR(1000), @Hex, 2),2);

RETURN
(
    Select @HexString
)

END
GO