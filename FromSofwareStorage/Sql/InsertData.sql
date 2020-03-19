-- Insert Folder data

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('ProgramFiles'
           ,NULL
           ,1);
GO

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('ProgramFilesX86'
           ,NULL
           ,1);
GO

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('ApplicationData'
           ,NULL
           ,1);
GO

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('LocalApplicationData'
           ,NULL
           ,1);
GO

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('MyDocuments'
           ,NULL
           ,1);
GO

INSERT INTO [Folder]
           ([Name]
           ,[FolderPath]
           ,[ReadOnly])
     VALUES
           ('Custom'
           ,NULL
           ,0);
GO

-- Insert game data

-- Dark souls3
INSERT INTO [Game]
           ([Name]
           ,[Directory]
           ,[DefaultFileName]
           ,[FileSearchPattern]
           ,[DefaultDirectory]
           ,[RootDirectory]
           ,[ChangeDate]
           ,[ReadOnly]
           ,[FolderId])
     VALUES
           ('DarkSouls 3'
           ,'DarkSoulsIII'
           ,'DS30000.sl2'
           ,'*.sl2'
           ,NULL
           ,NULL
           ,GETDATE()
           ,1
           ,3);
GO

-- Sekiro
INSERT INTO [Game]
           ([Name]
           ,[Directory]
           ,[DefaultFileName]
           ,[FileSearchPattern]
           ,[DefaultDirectory]
           ,[RootDirectory]
           ,[ChangeDate]
           ,[ReadOnly]
           ,[FolderId])
     VALUES
           ('Sekiro'
           ,'Sekiro'
           ,'S0000.sl2'
           ,'*.sl2'
           ,NULL
           ,NULL
           ,GETDATE()
           ,1
           ,3);
GO