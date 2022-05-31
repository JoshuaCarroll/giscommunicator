CREATE TABLE dbo.MapItems (
	ID int NOT NULL IDENTITY(1,1),
	DataSetID int NOT NULL,
	UID nvarchar(max) NULL DEFAULT (NULL),
	ReportedDatetime datetime NULL DEFAULT (NULL),
	Location geography NULL DEFAULT (NULL),
	Latitude decimal(10,7) NULL DEFAULT (NULL),
	Longitude decimal(10,7) NULL DEFAULT (NULL),
	LocationDescription nvarchar(max) NULL DEFAULT (NULL),
	Name nvarchar(255) NOT NULL,
	Description nvarchar(max) NOT NULL,
	Icon nvarchar(255) NULL DEFAULT (NULL),
	Recipient nvarchar(20) NULL DEFAULT (NULL)
);
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spGetMapItems @recipient varchar(max)
AS
BEGIN

	DECLARE @maxHours decimal(8,2) = 12.00;

	DECLARE @kml XML, @kmlout NVARCHAR(MAX), @localWx nvarchar(max), @severeWx nvarchar(max), @sitRep nvarchar(max), @checkIn nvarchar(max);

	exec dbo.spGetMapItems_data @recipient, @maxHours, 'Local Weather Report', @localWx OUTPUT;
	exec dbo.spGetMapItems_data @recipient, @maxHours, 'Severe WX Report', @severeWx OUTPUT;
	exec dbo.spGetMapItems_data @recipient, @maxHours, 'Field Situation Report', @sitRep OUTPUT;
	exec dbo.spGetMapItems_data @recipient, @maxHours, 'Winlink Check In', @checkIn OUTPUT;
	
	SELECT @kml = CAST(CONCAT('<?xml version="1.0" encoding="utf-16" ?><kml xmlns="http://www.opengis.net/kml/2.2"><Document>',
	'<Style id="hidelabel"><IconStyle><hotSpot x="15" y="0" xunits="pixels" yunits="pixels"/></IconStyle><LabelStyle><color>00ffffff</color></LabelStyle></Style><Style id="showlabel"><IconStyle><hotSpot x="15" y="0" xunits="pixels" yunits="pixels"/><color>FFffffff</color></IconStyle><LabelStyle><color>FFffffff</color></LabelStyle></Style><StyleMap id="iconStyle"><Pair><key>normal</key><styleUrl>#hidelabel</styleUrl></Pair><Pair><key>highlight</key><styleUrl>#showlabel</styleUrl></Pair></StyleMap>',
	@localWx, @severeWx, @sitRep, @checkIn,
	'</Document></kml>') AS XML)

	SET @kmlout = REPLACE(REPLACE(CAST(@kml AS NVARCHAR(MAX)), '&lt;', '<'), '&gt;', '>')
	SET @kmlout = REPLACE(@kmlout, 'utf-16', 'utf-8')

	SELECT @kmlout

END
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spGetMapItems_data @recipient nvarchar(max), @maxHoursOld decimal(8,2), @reportName nvarchar(max), @result nvarchar(max) OUTPUT
AS
BEGIN
	Declare @tempRecipients table (callsign nvarchar(20));

	IF @recipient <> 'ALL'
	BEGIN
		Delete from @tempRecipients;
		INSERT INTO @tempRecipients SELECT value as callsign FROM STRING_SPLIT(@recipient, ',')
	END

 	Set @result = Concat('<Folder><name>', @reportName, '</name><open>0</open>', (Select '#iconStyle' as styleUrl, name, Description as description, 
		Concat(Longitude, ',', Latitude, ',0') as [Point/coordinates], 
		Concat('http://aa5jc.com/map/icons/',Icon) as [Style/IconStyle/Icon/href], 
		Concat(dbo.IntToStr(100-(DateDiff(hour, ReportedDatetime, GetUTCDate()) / @maxHoursOld * 255)), 'ffffff') as [Style/IconStyle/color] 
		from MapItems 
		Where
		ReportedDateTime > DateAdd(Hour, -1*@maxHoursOld, GetUTCDate()) and 
		ReportedDateTime < GetUTCDate() and name = @reportName and 
			(
				(Recipient in (Select callsign from @tempRecipients)) 
				OR
				(1=
				CASE @recipient
					WHEN 'ALL' then 1
					Else 0
				End
				)
			)
		Order by Name FOR XML PATH('Placemark'), ELEMENTS),
		'</Folder>')
END
GO

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

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE FUNCTION PercentToHexString
(
	@i integer
)
RETURNS char(2)
AS
BEGIN
	DECLARE @percentage decimal(5,2) = @i/100;
	DECLARE @Hex varbinary(32) = CONVERT(varbinary, @percentage * 255 ), @HexString char(2) = NULL 
	SELECT @HexString = Right(CONVERT(VARCHAR(1000), @Hex, 2),2);

RETURN
(
    Select @HexString
)

END
GO