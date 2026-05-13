IF NOT EXISTS (
    SELECT 1
    FROM Posts
    WHERE UserId = 1 AND Content = N'Welcome to SocialMediaApp!' AND IsDeleted = 0
)
BEGIN
    INSERT INTO Posts (UserId, Content, Image, CreatedAt, IsDeleted)
    VALUES (1, N'Welcome to SocialMediaApp!', NULL, SYSUTCDATETIME(), 0);
END;

IF EXISTS (SELECT 1 FROM Posts WHERE Id = 1)
AND NOT EXISTS (
    SELECT 1
    FROM Comments
    WHERE UserId = 1 AND PostId = 1 AND Content = N'First comment sample' AND IsDeleted = 0
)
BEGIN
    INSERT INTO Comments (UserId, PostId, Content, CreatedAt, IsDeleted)
    VALUES (1, 1, N'First comment sample', SYSUTCDATETIME(), 0);
END;

DECLARE @SeedUserId INT = (
    SELECT TOP 1 Id
    FROM Users
    WHERE Email = 'seeduser@example.com'
);

IF @SeedUserId IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM Posts
    WHERE UserId = @SeedUserId AND Content = N'我是貼文' AND IsDeleted = 0
)
BEGIN
    INSERT INTO Posts (UserId, Content, Image, CreatedAt, IsDeleted)
    VALUES (@SeedUserId, N'我是貼文', NULL, SYSUTCDATETIME(), 0);
END;

DECLARE @SeedPostId INT = (
    SELECT TOP 1 Id
    FROM Posts
    WHERE UserId = @SeedUserId AND Content = N'我是貼文' AND IsDeleted = 0
    ORDER BY Id DESC
);

IF @SeedUserId IS NOT NULL
AND @SeedPostId IS NOT NULL
AND NOT EXISTS (
    SELECT 1
    FROM Comments
    WHERE UserId = @SeedUserId AND PostId = @SeedPostId AND Content = N'我是留言' AND IsDeleted = 0
)
BEGIN
    INSERT INTO Comments (UserId, PostId, Content, CreatedAt, IsDeleted)
    VALUES (@SeedUserId, @SeedPostId, N'我是留言', SYSUTCDATETIME(), 0);
END;
