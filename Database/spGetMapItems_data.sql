SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE spGetMapItems_data @recipient nvarchar(max), @maxHoursOld decimal(8,2), @reportName nvarchar(max), @result nvarchar(max) OUTPUT
AS
BEGIN
	Declare @tempRecipients table (callsign nvarchar(20));
	Declare @maxMinutesOld decimal(15,2) = @maxHoursOld*60;

	IF @recipient <> 'ALL'
	BEGIN
		Delete from @tempRecipients;
		INSERT INTO @tempRecipients SELECT value as callsign FROM STRING_SPLIT(@recipient, ',')
	END

 	Set @result = Concat('<Folder><name>', @reportName, '</name><open>0</open>', (Select '#iconStyle' as styleUrl, name, Description as description, 
		Concat(Longitude, ',', Latitude, ',0') as [Point/coordinates], 
		Concat('http://aa5jc.com/map/icons/',Icon) as [Style/IconStyle/Icon/href], 
		Concat(dbo.IntToStr(((@maxMinutesOld - DateDiff(minute, ReportedDatetime, GetUTCDate())) / @maxMinutesOld) * 255), 'ffffff') as [Style/IconStyle/color] 
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