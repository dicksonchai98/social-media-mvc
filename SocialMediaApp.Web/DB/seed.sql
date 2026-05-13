INSERT INTO Users (UserName, Email, PasswordHash, PhoneNumber, CreatedAt)
VALUES ('Admin', 'admin@example.com', 'seeded-password-hash', '0999999999', SYSUTCDATETIME());

INSERT INTO Users (UserName, Email, PasswordHash, PhoneNumber, CreatedAt)
VALUES ('SeedUser', 'seeduser@example.com', 'seeded-password-hash', '0911111111', SYSUTCDATETIME());
