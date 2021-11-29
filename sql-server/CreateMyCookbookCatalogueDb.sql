USE Master
GO
-- ================================================
-- Author:		einajade-forest
-- Create date: 24-NOV-2021
-- Description:	Sample database for My Cookbook Catalogue App
-- ================================================

CREATE DATABASE MyCookbookCatalogueDb
GO

USE MyCookbookCatalogueDb
GO

-- ================================================
--                  CREATE TABLES
-- ================================================

CREATE TABLE ShelfLocation
(
	LocationId INT IDENTITY (1,1) PRIMARY KEY,
	ShelfLocation NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Tag
(
	TagId INT IDENTITY(1,1) PRIMARY KEY,
	TagName NVARCHAR(25) NOT NULL UNIQUE
);

CREATE TABLE Cookbook
(
	ISBN13 NCHAR (13) PRIMARY KEY,
	CHECK (ISBN13 LIKE '[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'),
	Title NVARCHAR (150) NOT NULL,
	Contributor NVARCHAR (150) NOT NULL,
	ShelfLocation_Ref INT NOT NULL CONSTRAINT FK_ShelfLocCookbook FOREIGN KEY (ShelfLocation_Ref) REFERENCES ShelfLocation(LocationId)
);

CREATE TABLE Recipe
(
	RecipeId INT IDENTITY(1,1) PRIMARY KEY,
	RecipeName NVARCHAR (150) NOT NULL,
	PageNo NVARCHAR (10) NOT NULL,
	Cookbook_Ref NCHAR(13) NOT NULL CONSTRAINT FK_CookbookRecipe FOREIGN KEY (Cookbook_Ref) REFERENCES Cookbook(ISBN13)
);

CREATE TABLE RecipeTag
(
	Tag_Ref INT FOREIGN KEY REFERENCES Tag(TagId),
	Recipe_Ref INT FOREIGN KEY REFERENCES Recipe(RecipeId),
	CONSTRAINT PK_TaggedRecipe PRIMARY KEY (Tag_Ref, Recipe_Ref)
);

GO

-- ================================================
--             CREATE STORED PROCEDURES
-- ================================================

CREATE PROCEDURE sp_DeleteCookbook
	@isbn13 NCHAR(13)
AS
BEGIN
	DELETE FROM Cookbook WHERE ISBN13 = @isbn13;
END
GO

CREATE PROCEDURE sp_DeleteLocation
	@id NVARCHAR(50)
AS
BEGIN
	DELETE FROM ShelfLocation WHERE LocationId = @id;
END
GO

CREATE PROCEDURE sp_DeleteRecipe
	@id INT
AS
BEGIN
	DELETE FROM Recipe WHERE RecipeId = @id;
END
GO

CREATE PROCEDURE sp_DeleteRecipeTags
	@recipe INT
AS
BEGIN
	DELETE FROM RecipeTag WHERE Recipe_Ref = @recipe

END
GO

CREATE PROCEDURE sp_DeleteTag
	@id INT
AS
BEGIN
	DELETE FROM Tag WHERE TagId = @id;
END
GO


CREATE PROCEDURE sp_InsertCookbook
	@isbn13 NCHAR(13),
	@title NVARCHAR(150),
	@contributor NVARCHAR(150),
	@location INT
AS
BEGIN
	INSERT INTO Cookbook(ISBN13, Title, Contributor, ShelfLocation_Ref)
	VALUES (@isbn13, @title, @contributor, @location)
END
GO

CREATE PROCEDURE sp_InsertRecipeTag
	@tag NVARCHAR(25),
	@recipe INT
AS
BEGIN
	INSERT INTO RecipeTag (Tag_Ref, Recipe_Ref)
		VALUES
			(@tag, @recipe)
END
GO


CREATE PROCEDURE sp_SelectCookbooks
AS
BEGIN
	SELECT Cookbook.ISBN13, Cookbook.Title, Cookbook.Contributor, Cookbook.ShelfLocation_Ref, ShelfLocation.ShelfLocation 
	FROM Cookbook
		JOIN ShelfLocation ON Cookbook.ShelfLocation_Ref = ShelfLocation.LocationId
END
GO

CREATE PROCEDURE sp_SelectLocations
AS
BEGIN
	SELECT * FROM ShelfLocation
END
GO

CREATE PROCEDURE sp_SelectMaxRecipeId
AS
BEGIN
	SELECT MAX(RecipeId) AS LastUsedId
	FROM Recipe
END
GO

CREATE PROCEDURE sp_SelectRecipeById
	@id INT
AS
BEGIN
	SELECT Recipe.RecipeId, Recipe.RecipeName, Recipe.PageNo, Cookbook.Title, ShelfLocation.ShelfLocation
	FROM Recipe
		JOIN Cookbook ON Recipe.Cookbook_Ref = Cookbook.ISBN13
		JOIN ShelfLocation ON Cookbook.ShelfLocation_Ref = ShelfLocation.LocationId
	WHERE Recipe.RecipeId = @id;
END
GO

CREATE PROCEDURE sp_SelectRecipesByCookbook
	@isbn13 NCHAR(13)
AS
BEGIN
	SELECT Recipe.RecipeId, Recipe.RecipeName, Recipe.PageNo, Cookbook.Title, ShelfLocation.ShelfLocation
	FROM Recipe
		JOIN Cookbook ON Recipe.Cookbook_Ref = Cookbook.ISBN13
		JOIN ShelfLocation ON Cookbook.ShelfLocation_Ref = ShelfLocation.LocationId
	WHERE Recipe.Cookbook_Ref = @isbn13
END
GO

CREATE PROCEDURE sp_SelectRecipesByKeyword
	@keyword NVARCHAR(150)
AS
BEGIN
	SELECT Recipe.RecipeId, Recipe.RecipeName, Recipe.PageNo, Cookbook.Title, ShelfLocation.ShelfLocation
	FROM Recipe
		JOIN Cookbook ON Recipe.Cookbook_Ref = Cookbook.ISBN13
		JOIN ShelfLocation ON Cookbook.ShelfLocation_Ref = ShelfLocation.LocationId
	WHERE Recipe.RecipeName LIKE '%'+@keyword+'%'
END
GO

CREATE PROCEDURE sp_SelectRecipesByTag
	@tag INT
AS
BEGIN
	SELECT Recipe_Ref
	FROM RecipeTag
	WHERE Tag_Ref = @tag
END
GO

CREATE PROCEDURE sp_SelectTags
AS
BEGIN
	SELECT * FROM Tag
END
GO

CREATE PROCEDURE sp_SelectTagsByRecipe
	@recipeId INT
AS
BEGIN
	SELECT RecipeTag.Tag_Ref, Tag.TagName 
	FROM RecipeTag
		JOIN Tag ON RecipeTag.Tag_Ref = Tag.TagId
	WHERE Recipe_Ref = @recipeId
END
GO


CREATE PROCEDURE sp_UpdateCookbook
	@isbn13 NCHAR(13),
	@title NVARCHAR(150),
	@contributor NVARCHAR(150),
	@location INT
AS
BEGIN
	UPDATE Cookbook SET Title = @title, Contributor = @contributor, ShelfLocation_Ref = @location WHERE ISBN13 = @isbn13;
END
GO


CREATE PROCEDURE sp_UpsertLocation
	@id INT,
	@location NVARCHAR(50)
AS
IF (@id = 0)
BEGIN
	INSERT INTO ShelfLocation(ShelfLocation)
	VALUES (@location)
END

ELSE
BEGIN
	UPDATE ShelfLocation SET ShelfLocation = @location
	WHERE LocationId = @id
END
GO

CREATE PROCEDURE sp_UpsertRecipe
	@id INT,
	@name NVARCHAR(150),
	@page NVARCHAR(10),
	@isbn13 NCHAR(13)
AS

IF (@id = 0)
BEGIN
	INSERT INTO Recipe(RecipeName, PageNo, Cookbook_Ref)
		VALUES (@name, @page, @isbn13)
END

ELSE
BEGIN
		UPDATE Recipe SET RecipeName = @name, PageNo = @page, Cookbook_Ref = @isbn13
			WHERE RecipeId = @id;	
END
GO

CREATE PROCEDURE sp_UpsertTag
	@id INT,
	@tag NVARCHAR(25)
AS
IF (@id = 0)
BEGIN
	INSERT INTO Tag(TagName)
	VALUES (@tag)
END

ELSE
BEGIN
	UPDATE Tag SET TagName = @tag
	WHERE TagId = @id

END
GO

-- ================================================
--                    TEST DATA
-- ================================================

INSERT INTO ShelfLocation (ShelfLocation)
VALUES
	('LIVING ROOM'),
	('KITCHEN'),
	('STUDY');

INSERT INTO Tag (TagName)
VALUES
	('SAVOURY'),
	('SWEET');

INSERT INTO Cookbook (ISBN13, Title, Contributor, ShelfLocation_Ref)
VALUES
	('9780733636837','Green Soups','Green, Fern',1),
	('9781863965941','Fast Pasta', 'The Australian Women''s Weekly',1),
	('9780007275946','Baking Made Easy','Pascale, Lorraine',1);

INSERT INTO Recipe (RecipeName, PageNo, Cookbook_Ref)
VALUES
	('Carrot, Ginger & Tofu Soup','14','9780733636837'),
	('Spiced Butternut Pumpkin & Lemon Soup','16','9780733636837'),
	('Butternut Pumpkin & Carrot Soup','18','9780733636837'),
	('Mediterranean pasta salad','14','9781863965941'),
	('Artichoke pasta salad','17','9781863965941');

INSERT INTO RecipeTag (Tag_Ref, Recipe_Ref)
VALUES
	(1,1),
	(1,2),
	(1,3),
	(1,4),
	(1,5);

GO
