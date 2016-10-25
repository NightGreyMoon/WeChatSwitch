CREATE TABLE [dbo].[WechatLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LogContent] NVARCHAR(MAX) NULL, 
    [LogTime] DATETIME NULL
)
