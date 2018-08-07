CREATE FUNCTION [dbo].[nop_splitstring_to_table]
(
    @string NVARCHAR(MAX),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(MAX)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO



CREATE FUNCTION [dbo].[nop_getnotnullnotempty]
(
    @p1 nvarchar(max) = null, 
    @p2 nvarchar(max) = null
)
RETURNS nvarchar(max)
AS
BEGIN
    IF @p1 IS NULL
        return @p2
    IF @p1 =''
        return @p2

    return @p1
END
GO



CREATE FUNCTION [dbo].[nop_getprimarykey_indexname]
(
    @table_name nvarchar(1000) = null
)
RETURNS nvarchar(1000)
AS
BEGIN
	DECLARE @index_name nvarchar(1000)

    SELECT @index_name = i.name
	FROM sys.tables AS tbl
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
	WHERE (i.is_unique=1 and i.is_disabled=0) and (tbl.name=@table_name)

    RETURN @index_name
END
GO


CREATE FUNCTION [dbo].[nop_padright]
(
    @source INT, 
    @symbol NVARCHAR(MAX), 
    @length INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN RIGHT(REPLICATE(@symbol, @length)+ RTRIM(CAST(@source AS NVARCHAR(MAX))), @length)
END
GO

CREATE PROCEDURE [dbo].[FullText_IsSupported]
AS
BEGIN	
	EXEC('
	SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END as Value')
END
GO



CREATE PROCEDURE [dbo].[FullText_Enable]
AS
BEGIN
	--create catalog
	EXEC('
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		CREATE FULLTEXT CATALOG [nopCommerceFullTextCatalog] AS DEFAULT')
	
	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		CREATE FULLTEXT INDEX ON [Product]([Name], [ShortDescription], [FullDescription])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('Product') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
	
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('LocalizedProperty') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		CREATE FULLTEXT INDEX ON [ProductTag]([Name])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('ProductTag') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO



CREATE PROCEDURE [dbo].[FullText_Disable]
AS
BEGIN
	EXEC('
	--drop indexes
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		DROP FULLTEXT INDEX ON [Product]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		DROP FULLTEXT INDEX ON [LocalizedProperty]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		DROP FULLTEXT INDEX ON [ProductTag]
	')

	--drop catalog
	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		DROP FULLTEXT CATALOG [nopCommerceFullTextCatalog]
	')
END
GO


CREATE PROCEDURE [dbo].[LanguagePackImport]
(
	@LanguageId int,
	@XmlPackage xml,
	@UpdateExistingResources bit
)
AS
BEGIN
	IF EXISTS(SELECT * FROM [Language] WHERE [Id] = @LanguageId)
	BEGIN
		CREATE TABLE #LocaleStringResourceTmp
			(
				[LanguageId] [int] NOT NULL,
				[ResourceName] [nvarchar](200) NOT NULL,
				[ResourceValue] [nvarchar](MAX) NOT NULL
			)

		INSERT INTO #LocaleStringResourceTmp (LanguageId, ResourceName, ResourceValue)
		SELECT	@LanguageId, nref.value('@Name', 'nvarchar(200)'), nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageId, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName))
			BEGIN
				IF (@UpdateExistingResources = 1)
				BEGIN
					UPDATE [LocaleStringResource]
					SET [ResourceValue]=@ResourceValue
					WHERE LanguageId=@LanguageId AND ResourceName=@ResourceName
				END
			END
			ELSE 
			BEGIN
				INSERT INTO [LocaleStringResource]
				(
					[LanguageId],
					[ResourceName],
					[ResourceValue]
				)
				VALUES
				(
					@LanguageId,
					@ResourceName,
					@ResourceValue
				)
			END
			
			
			FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
			END
		CLOSE cur_localeresource
		DEALLOCATE cur_localeresource

		DROP TABLE #LocaleStringResourceTmp
	END
END
GO


CREATE PROCEDURE [dbo].[DeleteGuests]
(
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (UserId int)
		
	INSERT #tmp_guests (UserId)
	SELECT [Id] FROM [User] c with (NOLOCK)
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
	AND

	--guests only
	(EXISTS(SELECT 1 FROM [User_UserRole_Mapping] ccrm with (NOLOCK) inner join [User] with (NOLOCK) on ccrm.[User_Id]=c.[Id] inner join [UserRole] cr with (NOLOCK) on cr.[Id]=ccrm.[UserRole_Id] WHERE cr.[SystemName] = N'Guests'))
	AND
	--no orders
	(NOT EXISTS(SELECT 1 FROM [Order] o with (NOLOCK) inner join [User] with (NOLOCK) on o.[UserId]=c.[Id]))
	AND
	--no blog comments
	(NOT EXISTS(SELECT 1 FROM [BlogComment] bc with (NOLOCK) inner join [User] with (NOLOCK) on bc.[UserId]=c.[Id]))
	AND
	--no news comments
	(NOT EXISTS(SELECT 1 FROM [NewsComment] nc  with (NOLOCK)inner join [User] with (NOLOCK) on nc.[UserId]=c.[Id]))
	AND
	--no product reviews
	(NOT EXISTS(SELECT 1 FROM [ProductReview] pr with (NOLOCK) inner join [User] with (NOLOCK) on pr.[UserId]=c.[Id]))
	AND
	--no product reviews helpfulness
	(NOT EXISTS(SELECT 1 FROM [ProductReviewHelpfulness] prh with (NOLOCK) inner join [User] with (NOLOCK) on prh.[UserId]=c.[Id]))
	AND
	--no poll voting
	(NOT EXISTS(SELECT 1 FROM [PollVotingRecord] pvr with (NOLOCK) inner join [User] with (NOLOCK) on pvr.[UserId]=c.[Id]))
	AND
	--no forum topics 
	(NOT EXISTS(SELECT 1 FROM [Forums_Topic] ft with (NOLOCK) inner join [User] with (NOLOCK) on ft.[UserId]=c.[Id]))
	AND
	--no forum posts 
	(NOT EXISTS(SELECT 1 FROM [Forums_Post] fp with (NOLOCK) inner join [User] with (NOLOCK) on fp.[UserId]=c.[Id]))
	AND
	--no system accounts
	(c.IsSystemAccount = 0)
	
	--delete guests
	DELETE [User]
	WHERE [Id] IN (SELECT [UserId] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityId] IN (SELECT [UserId] FROM #tmp_guests))
	AND
	([KeyGroup] = N'User')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO

