SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE genBasicKml_data @maxHoursOld decimal(5,2), @reportName nvarchar(max), @iconUrl nvarchar(max), @result nvarchar(max) OUTPUT
AS
BEGIN

 Set @result = Concat('<Folder><name>', @reportName, '</name><open>0</open>', (Select '#iconStyle' as styleUrl, name, Description as description, 
		Concat(Longitude, ',', Latitude, ',0') as [Point/coordinates], 
		@iconUrl as [Style/IconStyle/Icon/href], 
		Concat(dbo.IntToStr(100-(DateDiff(hour, ReportedDatetime, GetDate()) / @maxHoursOld * 100)), 'ffffff') as [Style/IconStyle/color] 
		from MapItems 
		Where ReportedDateTime > DateAdd(Hour, -1*@maxHoursOld, GetDate()) and ReportedDateTime < GetDate() and name = @reportName 
		Order by Name FOR XML PATH('Placemark'), ELEMENTS),
		'</Folder>')

END
GO