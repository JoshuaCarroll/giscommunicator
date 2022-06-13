SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
CREATE PROCEDURE [dbo].[spCreateMapItem] 
	@DataSet nvarchar(5), @UID nvarchar(max), @Latitude nvarchar(max), @Longitude nvarchar(max), @LocationDescription nvarchar(max), 
	@Name nvarchar(max), @Description nvarchar(max), @Icon nvarchar(max), @ReportedDateTime nvarchar(max), @Recipient nvarchar(20), 
	@FormData xml
AS   
BEGIN

	Declare @exists int;
	Select @exists = Count(UID) from MapItems where UID = @UID and cast(formdata as varchar(max)) != null;;

	if @exists = 0
	Begin
		-- It could be that there is a record, but the formdata is empty. So...
		Delete from MapItems where UID = @UID;

		if @Icon = ''
		BEGIN
			Select @Icon = Case @Name
			When 'Local Weather Report' Then 'cloudy.png'
			When 'Winlink Check In' Then 'person.png'
			When 'Severe WX Report' Then 'storm.png'
			When 'Field Situation Report' Then 'radar.png'
			End
		END

		DECLARE @Location geography = geography::Point(@latitude,@longitude, 4326);
		Insert into MapItems (DataSetID, UID, Location, LocationDescription, Name, Description, Icon, ReportedDateTime, Latitude, Longitude, Recipient, FormData) VALUES 
							(@DataSet, @UID, @Location, @LocationDescription, @Name, @Description, @Icon, @ReportedDateTime, @Latitude, @Longitude, @Recipient, @FormData);
	End

END;
GO