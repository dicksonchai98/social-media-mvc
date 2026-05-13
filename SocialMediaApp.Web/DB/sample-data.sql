INSERT INTO Posts (UserId, Content, Image, CreatedAt, IsDeleted)
VALUES (1, N'Welcome to SocialMediaApp!', NULL, SYSUTCDATETIME(), 0);

INSERT INTO Comments (UserId, PostId, Content, CreatedAt, IsDeleted)
VALUES (1, 1, N'First comment sample', SYSUTCDATETIME(), 0);

DECLARE @SeedUserId INT = (
    SELECT TOP 1 Id
    FROM Users
    WHERE Email = 'seeduser@example.com'
);

INSERT INTO Posts (UserId, Content, Image, CreatedAt, IsDeleted)
VALUES (@SeedUserId, N'我是貼文', NULL, SYSUTCDATETIME(), 0);

DECLARE @SeedPostId INT = SCOPE_IDENTITY();

INSERT INTO Comments (UserId, PostId, Content, CreatedAt, IsDeleted)
VALUES (@SeedUserId, @SeedPostId, N'我是留言', SYSUTCDATETIME(), 0);
