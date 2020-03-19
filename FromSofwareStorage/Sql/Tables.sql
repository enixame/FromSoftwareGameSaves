﻿-- Folder
CREATE TABLE [Folder]
(
    [Id] INT IDENTITY PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL,
	[FolderPath] NVARCHAR(256) NULL,
	[ReadOnly] BIT NOT NULL DEFAULT(0)
)

GO

-- Game
CREATE TABLE [Game]
(
    [Name] NVARCHAR(30) PRIMARY KEY,
	[Directory] NVARCHAR(30) NOT NULL,
	[DefaultFileName] NVARCHAR(100) NOT NULL,
	[FileSearchPattern] NVARCHAR(50) NOT NULL,
	[DefaultDirectory] NVARCHAR(100) NULL,
	[RootDirectory] NVARCHAR(256) NULL,
	[ChangeDate] DATETIME NOT NULL,
	[ReadOnly] BIT NOT NULL DEFAULT(0),
	[FolderId] INT NULL
)

GO

ALTER TABLE [Game]
    ADD CONSTRAINT FK_Game_Folder FOREIGN KEY ([FolderId]) 
	REFERENCES [Folder] ([Id])
	ON UPDATE CASCADE

GO