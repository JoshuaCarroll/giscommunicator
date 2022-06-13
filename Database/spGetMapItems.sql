SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spGetMapItems @recipient varchar(max)
AS
BEGIN

	DECLARE @maxHours decimal(8,2) = 24.00;

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