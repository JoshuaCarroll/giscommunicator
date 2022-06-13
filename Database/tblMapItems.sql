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
	Recipient nvarchar(20) NULL DEFAULT (NULL),
	FormData xml NULL DEFAULT ('NULL')
);
GO

