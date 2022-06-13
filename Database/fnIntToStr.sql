SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION IntToStr
(
	@i integer
)
RETURNS char(2)
AS
BEGIN
	-- DECLARE @i integer = 255
	DECLARE @Hex varbinary(32) = CONVERT(varbinary, @i), @HexString char(2) = NULL 
	SELECT  @HexString = Right(CONVERT(VARCHAR(1000), @Hex, 2),2);

RETURN
(
    Select @HexString
)

END
GO