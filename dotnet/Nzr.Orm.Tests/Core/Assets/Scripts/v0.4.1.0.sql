/****** v0.4.1.0 ******/
DROP TABLE IF EXISTS [security].[user_type]
GO

CREATE TABLE [security].[user_type]
(
	[id_user_type] [int] NOT NULL IDENTITY(1,1),
	[code] [int] UNIQUE NOT NULL,
	[description] [varchar](25) NOT NULL

	CONSTRAINT [PK_user_type_id_user_type] PRIMARY KEY CLUSTERED
	(
		[id_user_type] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

INSERT INTO  [security].[user_type] (code, description) VALUES (1, 'INTERNAL');
INSERT INTO  [security].[user_type] (code, description) VALUES (2, 'EXTERNAL');
GO

ALTER TABLE [security].[application_user]  WITH CHECK ADD  CONSTRAINT [FK_user_type_code] FOREIGN KEY([user_type_code])
REFERENCES [security].[user_type] ([code])
GO

ALTER TABLE [security].[application_user] CHECK CONSTRAINT [FK_user_type_code]
GO