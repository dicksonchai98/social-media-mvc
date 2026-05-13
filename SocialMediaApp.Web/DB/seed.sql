IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@example.com')
BEGIN
    INSERT INTO Users (UserName, Email, PasswordHash, PhoneNumber, CreatedAt)
    VALUES ('Admin', 'admin@example.com', 'seeded-password-hash', '0999999999', SYSUTCDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'seeduser@example.com')
BEGIN
    INSERT INTO Users (UserName, Email, PasswordHash, PhoneNumber, CreatedAt)
    VALUES ('SeedUser', 'seeduser@example.com', 'seeded-password-hash', '0911111111', SYSUTCDATETIME());
END;
