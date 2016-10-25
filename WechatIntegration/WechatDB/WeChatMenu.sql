CREATE TABLE [dbo].[WechatMenu]
(
	[ButtonId] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [Key] NVARCHAR(50) NULL, 
    [Url] NVARCHAR(2000) NULL, 
    [ParentButtonId] INT NULL
)
