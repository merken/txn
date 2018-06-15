IF NOT EXISTS (SELECT * FROM dbo.sysobjects where id = object_id(N'dbo.[MyTable]') and OBJECTPROPERTY(id, N'IsTable') = 1)
BEGIN
	CREATE TABLE [dbo].[MyTable](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedTimeStamp] [datetimeoffset](7) NOT NULL,
	[Description] [varchar](MAX) NOT NULL
	CONSTRAINT [PK_MyTable] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END