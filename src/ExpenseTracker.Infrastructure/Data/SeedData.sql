-- Create stored procedure to create default categories for a user
CREATE OR ALTER PROCEDURE [dbo].[CreateDefaultCategoriesForUser]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    -- Check if user exists and doesn't already have default categories
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE UserId = @UserId AND IsDefault = 1)
    BEGIN
        INSERT INTO Categories (Id, Name, Description, Icon, Color, Type, IsDefault, UserId, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
        VALUES 
            (NEWID(), 'Groceries', 'Food and household items', 'shopping_cart', '#4CAF50', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Transportation', 'Car, bus, and other transport expenses', 'directions_car', '#2196F3', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Entertainment', 'Movies, games, and fun activities', 'movie', '#9C27B0', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Utilities', 'Electricity, water, and internet bills', 'power', '#FF9800', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Healthcare', 'Medical expenses and medications', 'local_hospital', '#F44336', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Dining Out', 'Restaurants and cafes', 'restaurant', '#FF5722', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Shopping', 'Clothing and personal items', 'shopping_bag', '#E91E63', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL),
            (NEWID(), 'Education', 'Books, courses, and training', 'school', '#3F51B5', 0, 1, @UserId, GETDATE(), 'SYSTEM', NULL, NULL);
    END
END
GO

-- To create test data, first create a test user and then add expenses
DECLARE @TestUserId UNIQUEIDENTIFIER = NEWID();

-- Create test user
INSERT INTO Users (Id, Email, Username, PasswordHash, FirstName, LastName, PreferredCurrency, EmailNotificationsEnabled, CreatedAt, CreatedBy)
VALUES (
    @TestUserId,
    'test@example.com',
    'testuser',
    'AQAAAAIAAYagAAAAELbhHXRz6JZBMEYEYCyqZuJtF4QpHpOk4YAGmQLivKFYmKHB/gjWJ/t4LirGkrQUeA==', -- password: Test@123
    'Test',
    'User',
    'USD',
    0,
    GETDATE(),
    'SYSTEM'
);

-- Create default categories for test user
EXEC CreateDefaultCategoriesForUser @TestUserId;

-- Sample expenses for different dates and categories
DECLARE @StartDate DATE = DATEADD(MONTH, -6, GETDATE())
DECLARE @EndDate DATE = GETDATE()
DECLARE @CurrentDate DATE = @StartDate

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Get a random category ID for this user
    DECLARE @CategoryId UNIQUEIDENTIFIER;
    SELECT TOP 1 @CategoryId = Id 
    FROM Categories 
    WHERE UserId = @TestUserId 
    ORDER BY NEWID();

    -- Daily expense
    INSERT INTO Expenses (Id, Amount, Date, Description, Type, Currency, CategoryId, UserId, Notes, IsRecurring, RecurrencePattern, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (
        NEWID(),
        ROUND(RAND() * (100 - 10) + 10, 2),
        @CurrentDate,
        'Daily expense',
        0,
        'USD',
        @CategoryId,
        @TestUserId,
        NULL,
        0,
        NULL,
        GETDATE(),
        'SYSTEM',
        NULL,
        NULL
    );

    -- Additional expenses on weekends
    IF DATEPART(WEEKDAY, @CurrentDate) IN (1, 7)
    BEGIN
        SELECT TOP 1 @CategoryId = Id 
        FROM Categories 
        WHERE UserId = @TestUserId AND Id != @CategoryId
        ORDER BY NEWID();

        INSERT INTO Expenses (Id, Amount, Date, Description, Type, Currency, CategoryId, UserId, Notes, IsRecurring, RecurrencePattern, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
        VALUES (
            NEWID(),
            ROUND(RAND() * (200 - 50) + 50, 2),
            @CurrentDate,
            'Weekend activity',
            0,
            'USD',
            @CategoryId,
            @TestUserId,
            NULL,
            0,
            NULL,
            GETDATE(),
            'SYSTEM',
            NULL,
            NULL
        );
    END

    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
END
