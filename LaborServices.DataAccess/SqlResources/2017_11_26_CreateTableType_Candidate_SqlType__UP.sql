Go
IF TYPE_ID(N'Candidate_SqlType') IS NULL
	CREATE TYPE [dbo].[Candidate_SqlType] AS TABLE(
		[Labor_VisaNumber] [nvarchar](max) NULL,
		[DateOfBirth] [datetime] NULL,
		[Profession] [nvarchar](max) NULL,
		[Country] [nvarchar](max) NULL,
		[PassportNumber] [nvarchar](max) NULL,
		[IssueDate] [datetime] NULL,
		[BlockVisaNo] [nvarchar](max) NULL,
		[Authorization] [nvarchar](max) NULL,
		[ProjectCode] [nvarchar](max) NULL,
		[ProjectName] [nvarchar](max) NULL,
		[FlightNumber] [nvarchar](max) NULL,
		[AirCompany] [nvarchar](max) NULL,
		[DepartureTime] [nvarchar](max) NULL,
		[ArrivalDate] [datetime] NULL,
		[ArrivalTime] [nvarchar](max) NULL,
		[ArrivalPlace] [nvarchar](max) NULL,
		[BasicSalary] [decimal](18, 2) NULL,
		[Foodallowance] [decimal](18, 2) NULL,
		[HousingAllowance] [decimal](18, 2) NULL,
		[OtherAllowance] [decimal](18, 2) NULL,
		[TransportationAllowance] [decimal](18, 2) NULL,
		[ExpectedArrivalDate] [datetime] NULL,
		[Name] [nvarchar](max) NULL
	);
GO
