
CREATE TABLE [mapping_template]
(
	[id_mapping_template] [uniqueidentifier] NOT NULL,
	[name] varchar(20) NOT NULL
	CONSTRAINT [PK_conversion__mapping_template__id_mapping_template] PRIMARY KEY CLUSTERED
	(
		[id_mapping_template] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TABLE [mapping_field]
(
	[id_mapping_field] [uniqueidentifier] NOT NULL,
	[id_mapping_template] [uniqueidentifier] NOT NULL,
	[name] varchar(20) NOT NULL
	CONSTRAINT [PK_conversion__mapping_field__id_mapping_field] PRIMARY KEY CLUSTERED
	(
		[id_mapping_field] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO


CREATE TABLE [mapping]
(
	[id_mapping] [uniqueidentifier] NOT NULL,
	[id_mapping_field_source] [uniqueidentifier] NOT NULL,
	[id_mapping_field_dest] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_conversion__mapping__id_mapping] PRIMARY KEY CLUSTERED
	(
		[id_mapping] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO