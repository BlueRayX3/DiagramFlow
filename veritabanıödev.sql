CREATE DATABASE DiagramFlowDB
GO

USE DiagramFlowDB
GO

CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(30) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    CanCreateProject BIT NOT NULL DEFAULT 0,
    CanDeleteProject BIT NOT NULL DEFAULT 0,
    CanManageUsers BIT NOT NULL DEFAULT 0
)

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    RoleID INT NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    AvatarURL NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastLoginAt DATETIME2 NULL
)

CREATE TABLE Projects (
    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
    OwnerID INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsPublic BIT NOT NULL DEFAULT 0,
    Settings NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
)

CREATE TABLE Diagrams (
    DiagramID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    CreatedBy INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    DiagramType NVARCHAR(30) NOT NULL CHECK (DiagramType IN ('ER', 'UMLClass', 'UMLSequence', 'Flowchart')),
    Content NVARCHAR(MAX) NOT NULL,
    Version INT NOT NULL DEFAULT 1 CHECK (Version >= 1),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL
)

CREATE TABLE Diagram_History (
    HistoryID INT IDENTITY(1,1) PRIMARY KEY,
    DiagramID INT NOT NULL,
    ChangedBy INT NOT NULL,
    PreviousContent NVARCHAR(MAX) NOT NULL,
    ChangeDescription NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
)

CREATE TABLE Collaborators (
    CollaboratorID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    UserID INT NOT NULL,
    Permission NVARCHAR(20) NOT NULL CHECK (Permission IN ('Owner', 'Editor', 'Viewer')),
    InvitedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    AcceptedAt DATETIME2 NULL
)

CREATE TABLE Diagram_Templates (
    TemplateID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    DiagramType NVARCHAR(30) NOT NULL CHECK (DiagramType IN ('ER', 'UMLClass', 'UMLSequence', 'Flowchart')),
    Content NVARCHAR(MAX) NOT NULL,
    IsSystemTemplate BIT NOT NULL DEFAULT 0,
    CreatedBy INT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
)

ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE NO ACTION

ALTER TABLE Projects ADD CONSTRAINT FK_Projects_Users FOREIGN KEY (OwnerID) REFERENCES Users(UserID) ON DELETE CASCADE

ALTER TABLE Diagrams ADD CONSTRAINT FK_Diagrams_Projects FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID) ON DELETE CASCADE

ALTER TABLE Diagrams ADD CONSTRAINT FK_Diagrams_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserID) ON DELETE NO ACTION

ALTER TABLE Diagram_History ADD CONSTRAINT FK_DiagramHistory_Diagrams FOREIGN KEY (DiagramID) REFERENCES Diagrams(DiagramID) ON DELETE CASCADE

ALTER TABLE Diagram_History ADD CONSTRAINT FK_DiagramHistory_Users FOREIGN KEY (ChangedBy) REFERENCES Users(UserID) ON DELETE NO ACTION

ALTER TABLE Collaborators ADD CONSTRAINT FK_Collaborators_Projects FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID) ON DELETE CASCADE

ALTER TABLE Collaborators ADD CONSTRAINT FK_Collaborators_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION

ALTER TABLE Diagram_Templates ADD CONSTRAINT FK_Templates_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserID) ON DELETE SET NULL

--test amaçlı seed datalar
INSERT INTO Roles (RoleName, Description, CanCreateProject, CanDeleteProject, CanManageUsers)
VALUES
('Admin', 'Sistem Yöneticisi - Tüm yetkilere sahip', 1, 1, 1),
('Editor', 'İçerik Düzenleyici - Proje oluşturabilir', 1, 1, 0),
('Viewer', 'Görüntüleyici - Sadece okuma yetkisi var', 0, 0, 0)

INSERT INTO Users (Username, Email, PasswordHash, RoleID, FirstName, LastName, IsActive)
VALUES
('admin_user', 'admin@diagramflow.com', 'hashed_password_123', 1, 'Sistem', 'Yönetici', 1),
('test_editor', 'editor@diagramflow.com', 'hashed_password_456', 2, 'Test', 'Editörü', 1);

INSERT INTO Projects (OwnerID, Name, Description, IsPublic)
VALUES
(1, 'DiagramFlow MVP', 'Uygulamanın ilk test projesi', 1)

INSERT INTO Diagrams (ProjectID, CreatedBy, Title, DiagramType, Content, Version)
VALUES
(1, 1, 'Örnek Veritabanı ER Diyagramı', 'ER', '{"nodes": [], "edges": []}', 1)


INSERT INTO Diagrams (ProjectID, CreatedBy, Title, DiagramType, Content, Version, UpdatedAt)
VALUES
(1, 1, 'Örnek Veritabanı ER Diyagramı', 'ER', '{"nodes": [], "edges": []}', 1, GETDATE())

INSERT INTO Diagram_Templates (Name, Description, DiagramType, Content, IsSystemTemplate)
VALUES
('Boş ER Diyagramı', 'Standart Varlık-İlişki (ER) şablonu', 'ER', '{}', 1),
('Boş UML Class', 'Standart UML Sınıf Diyagramı şablonu', 'UMLClass', '{}', 1)

INSERT INTO Collaborators (ProjectID, UserID, Permission, InvitedAt, AcceptedAt)
VALUES
(1, 2, 'Editor', GETDATE(), GETDATE())

INSERT INTO Diagram_History (DiagramID, ChangedBy, PreviousContent, ChangeDescription, CreatedAt)
VALUES
((SELECT TOP 1 DiagramID FROM Diagrams), 1, '{"nodes": [], "edges": []}', 'İlk boş şablon oluşturuldu.', DATEADD(MINUTE, -30, GETDATE())),
((SELECT TOP 1 DiagramID FROM Diagrams), 2, '{"nodes": [{"id": "n1", "label": "Users"}], "edges": []}', 'Users tablosu düğümü eklendi.', DATEADD(MINUTE, -10, GETDATE()))


ALTER TABLE Users ADD IsVIP BIT NOT NULL DEFAULT 0

CREATE TRIGGER trg_KullaniciVIPGuncelle
ON Projects
AFTER INSERT, DELETE
AS BEGIN
    UPDATE Users
    SET IsVIP = CASE 
        WHEN (SELECT COUNT(*) FROM Projects WHERE Projects.OwnerID = Users.UserID) > 5 THEN 1
        ELSE 0
END
END


SELECT * FROM Roles 
SELECT * FROM Users 
SELECT * FROM Projects 
SELECT * FROM Diagrams 
SELECT * FROM Diagram_History 
SELECT * FROM Collaborators 
SELECT * FROM Diagram_Templates

