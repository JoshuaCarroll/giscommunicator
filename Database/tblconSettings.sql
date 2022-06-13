CREATE TABLE dbo.IconSettings (
	ID int NOT NULL IDENTITY(1,1),
	Priority int NOT NULL,
	XQuery varchar(max) NOT NULL,
	SQLType varchar(max) NOT NULL,
	Operator nvarchar(2) NOT NULL DEFAULT ('='),
	CompareValue nvarchar(max) NOT NULL,
	Icon nvarchar(255) NOT NULL
);
GO

