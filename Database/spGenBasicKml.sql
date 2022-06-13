SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE genBasicKML
AS
BEGIN

	DECLARE @maxHours decimal(5,2) = 999.00;

	DECLARE @kml XML, @kmlout NVARCHAR(MAX), @localWx nvarchar(max), @severeWx nvarchar(max), @sitRep nvarchar(max), @checkIn nvarchar(max);

	exec dbo.genBasicKml_data @maxHours, 'Local Weather Report', 'http://aa5jc.com/icons/cloudy.png', @localWx OUTPUT;
	exec dbo.genBasicKml_data @maxHours, 'Severe WX Report', 'http://aa5jc.com/icons/storm.png', @severeWx OUTPUT;
	exec dbo.genBasicKml_data @maxHours, 'Field Situation Report', 'http://aa5jc.com/icons/radar.png', @sitRep OUTPUT;
	exec dbo.genBasicKml_data @maxHours, 'Winlink Check In', 'http://aa5jc.com/icons/person.png', @checkIn OUTPUT;
	
	SELECT @kml = CAST(CONCAT('<?xml version="1.0" encoding="utf-16" ?><kml xmlns="http://www.opengis.net/kml/2.2"><Document>',
	'<Style id="hidelabel"><IconStyle></IconStyle><LabelStyle><color>00ffffff</color></LabelStyle></Style><Style id="showlabel"><IconStyle><color>FFffffff</color></IconStyle><LabelStyle><color>FFffffff</color></LabelStyle></Style><StyleMap id="iconStyle"><Pair><key>normal</key><styleUrl>#hidelabel</styleUrl></Pair><Pair><key>highlight</key><styleUrl>#showlabel</styleUrl></Pair></StyleMap>',
	@localWx, @severeWx, @sitRep, @checkIn,
	'</Document></kml>') AS XML)

	SET @kmlout = REPLACE(REPLACE(CAST(@kml AS NVARCHAR(MAX)), '&lt;', '<'), '&gt;', '>')
	SET @kmlout = REPLACE(@kmlout, 'utf-16', 'utf-8')

	SELECT @kmlout
END;
GO