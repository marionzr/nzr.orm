/****** v0.2.0.0 ******/
CREATE database [nzr.orm.core]
GO

ALTER DATABASE [nzr.orm.core]
SET ALLOW_SNAPSHOT_ISOLATION ON

ALTER DATABASE [nzr.orm.core]
SET READ_COMMITTED_SNAPSHOT ON

USE [nzr.orm.core]
GO

USE [nzr.orm.core]
GO

/****** SCHEMAS ******/
IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'crm'))
BEGIN
    EXEC ('CREATE SCHEMA [crm]')
END

IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'security'))
BEGIN
    EXEC ('CREATE SCHEMA [security]')
END

IF (NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'audit'))
BEGIN
    EXEC ('CREATE SCHEMA [audit]')
END

/****** TABLES ******/

DROP TABLE IF EXISTS [crm].[state]
GO

CREATE TABLE [crm].[state]
(
	[id_state] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[name] [varchar](255) NULL,
	CONSTRAINT [PK_state_id_state] PRIMARY KEY CLUSTERED
	(
		[id_state] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

DROP TABLE IF EXISTS [crm].[city]
GO

CREATE TABLE [crm].[city](
	[id_city] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[id_state] [uniqueidentifier] NOT NULL,
	[name] [varchar](40) NOT NULL
	CONSTRAINT [PK_city_id_city] PRIMARY KEY CLUSTERED
	(
		[id_city] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

ALTER TABLE [crm].[city]  WITH CHECK ADD  CONSTRAINT [FK_city_state_id_state] FOREIGN KEY([id_state])
REFERENCES [crm].[state] ([id_state])
GO

ALTER TABLE [crm].[city] CHECK CONSTRAINT [FK_city_state_id_state]
GO

DROP TABLE IF EXISTS [crm].[address]
GO

CREATE TABLE [crm].[address](
	[id_address] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[address_line] [varchar](120) NOT NULL,
	[zip_code] [nchar](10) NOT NULL,
	[id_city] [uniqueidentifier] NOT NULL,
	[created_at] [datetime] NOT NULL DEFAULT GETDATE(),
	[updated_at] [datetime] NOT NULL DEFAULT GETDATE()
	CONSTRAINT [PK_address_id_address] PRIMARY KEY CLUSTERED
	(
		[id_address] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

ALTER TABLE [crm].[address]  WITH CHECK ADD  CONSTRAINT [FK_address_city_id_city] FOREIGN KEY([id_city])
REFERENCES [crm].[city] ([id_city])
GO

ALTER TABLE [crm].[address] CHECK CONSTRAINT [FK_address_city_id_city]
GO

CREATE TRIGGER tr_crm_address on [crm].[address]
AFTER UPDATE
AS
BEGIN
   SET nocount ON;
   UPDATE [crm].[address] SET updated_at = GETDATE()
   FROM  [crm].[address] u
   INNER JOIN inserted i ON u.id_address = i.id_address
END
GO

DROP TABLE IF EXISTS [security].[profile]
GO

CREATE TABLE [security].[profile]
(
	[id_profile] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[permissions] [varchar](255) NULL,
	[created_at] [datetime] NOT NULL DEFAULT GETDATE(),
	[updated_at] [datetime] NOT NULL DEFAULT GETDATE()
	CONSTRAINT [PK_profile_id_profile] PRIMARY KEY CLUSTERED
	(
		[id_profile] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

CREATE TRIGGER tr_security_profile on [security].[profile]
AFTER UPDATE
AS
BEGIN
   SET nocount ON;
   UPDATE [security].[profile] SET updated_at = GETDATE()
   FROM  [security].[profile] u
   INNER JOIN inserted i ON u.id_profile = i.id_profile
END
GO

DROP TABLE IF EXISTS [security].[application_user]
GO

CREATE TABLE [security].[application_user](
	[id_application_user] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[username] [nvarchar](120) NOT NULL,
	[password] [nvarchar](120) NOT NULL,
	[id_profile] [uniqueidentifier] NOT NULL,
	[created_at] [datetime] NOT NULL DEFAULT GETDATE(),
	[updated_at] [datetime] NOT NULL DEFAULT GETDATE()
	CONSTRAINT [PK_user_id_application_user] PRIMARY KEY CLUSTERED
	(
		[id_application_user] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

ALTER TABLE [security].[application_user]  WITH CHECK ADD  CONSTRAINT [FK_user_profile_id_profile] FOREIGN KEY([id_profile])
REFERENCES [security].[profile] ([id_profile])
GO

ALTER TABLE [security].[application_user] CHECK CONSTRAINT [FK_user_profile_id_profile]
GO

CREATE TRIGGER tr_security_user on [security].[application_user]
AFTER UPDATE
AS
BEGIN
   SET nocount ON;
   UPDATE [security].[application_user] SET updated_at = GETDATE()
   FROM  [security].[application_user] u
   INNER JOIN inserted i ON u.id_application_user = i.id_application_user
END
GO

DROP TABLE IF EXISTS [crm].[customer]
GO

CREATE TABLE [crm].[customer](
	[id_customer] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[email] [nvarchar](120) NOT NULL,
	[balance] [decimal](10,2),
	[id_application_user] [uniqueidentifier],
	[id_address] [uniqueidentifier],
	[characteristics] [text],
	[created_at] [datetime] NOT NULL DEFAULT GETDATE(),
	[updated_at] [datetime] NOT NULL DEFAULT GETDATE()
	CONSTRAINT [PK_customer_id_customer] PRIMARY KEY CLUSTERED
	(
		[id_customer] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO

ALTER TABLE [crm].[customer]  WITH CHECK ADD  CONSTRAINT [FK_customer_user_id_application_user] FOREIGN KEY([id_application_user])
REFERENCES [security].[application_user] ([id_application_user])
GO

ALTER TABLE [crm].[customer]  WITH CHECK ADD  CONSTRAINT [FK_customer_address_id_address] FOREIGN KEY([id_address])
REFERENCES [crm].[address] ([id_address])
GO


ALTER TABLE [crm].[customer] CHECK CONSTRAINT [FK_customer_user_id_application_user]
GO

ALTER TABLE [crm].[customer] CHECK CONSTRAINT [FK_customer_address_id_address]
GO

CREATE TRIGGER tr_crm_customer on [crm].[customer]
AFTER UPDATE
AS
BEGIN
   SET nocount ON;
   UPDATE [crm].[customer] SET updated_at = GETDATE()
   FROM  [crm].[customer] u
   INNER JOIN inserted i ON u.id_customer = i.id_customer
END
GO


DROP TABLE IF EXISTS [audit].[TBL_EVENT]
GO

CREATE TABLE [audit].[TBL_EVENT]
(
	[CLN_ID] [int] NOT NULL IDENTITY(1,1),
	[CLN_TLB_NAME] [varchar](255) NULL,
	[CLN_DATE] [datetime] NOT NULL DEFAULT GETDATE(),
	[CLN_DATA] [text] NULL,
	CONSTRAINT [PK_TBL_EVENT_CLN_ID] PRIMARY KEY CLUSTERED
	(
		[CLN_ID] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)
ON [PRIMARY]
GO


CREATE VIEW [crm].[vw_active_customer]
AS
SELECT crm.customer.id_customer, crm.customer.email, security.application_user.username
FROM     security.application_user INNER JOIN
                  crm.customer ON security.application_user.id_application_user = crm.customer.id_application_user
GO
