CREATE TABLE [dbo].[WechatMessage]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [FromUserName] NVARCHAR(255) NULL, 
    [ToUserName] NVARCHAR(255) NULL, 
    [CreateTime] DATETIME NULL, 
    [MsgType] NVARCHAR(50) NULL, 
    [Direction] NVARCHAR(50) NULL,
    [RawData] NVARCHAR(MAX) NULL
)
