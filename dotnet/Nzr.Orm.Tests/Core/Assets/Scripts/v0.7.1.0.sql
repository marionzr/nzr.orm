/****** v0.7.1.0 ******/
ALTER TABLE [crm].[customer] ADD [id_billing_address] [uniqueidentifier]
GO

ALTER TABLE [crm].[customer]  WITH CHECK ADD  CONSTRAINT [FK_customer_address_id_billing_address] FOREIGN KEY([id_billing_address])
REFERENCES [crm].[address] ([id_address])
GO

ALTER TABLE [crm].[customer] CHECK CONSTRAINT [FK_customer_address_id_address]
GO