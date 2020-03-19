-- Folder
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

-- Image
CREATE TABLE [Image]
(
	[Id] INT IDENTITY PRIMARY KEY,
	[GameName] NVARCHAR(30) NOT NULL,
	[ImageFile] IMAGE NOT NULL
)

GO

-- Game foreign key
ALTER TABLE [Game]
    ADD CONSTRAINT FK_Game_Folder FOREIGN KEY ([FolderId]) 
	REFERENCES [Folder] ([Id])
	ON UPDATE CASCADE

GO

-- Image foreign key
ALTER TABLE [Image]
    ADD CONSTRAINT FK_Image_Game FOREIGN KEY ([GameName]) 
	REFERENCES [Game] ([Name])
	ON UPDATE CASCADE

GO