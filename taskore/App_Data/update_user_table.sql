-- Script pentru actualizarea tabelei Users cu noile coloane pentru profilul utilizatorului

-- Adaugă coloana headline
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'headline')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [headline] NVARCHAR(255) NULL;
END

-- Adaugă coloana about
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'about')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [about] NVARCHAR(MAX) NULL;
END

-- Adaugă coloana skills
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'skills')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [skills] NVARCHAR(MAX) NULL;
END

-- Adaugă coloana phone
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'phone')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [phone] NVARCHAR(50) NULL;
END

-- Adaugă coloana location
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'location')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [location] NVARCHAR(255) NULL;
END

-- Adaugă coloana website
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'website')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [website] NVARCHAR(255) NULL;
END

-- Adaugă coloana preferred_project_types
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'preferred_project_types')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [preferred_project_types] NVARCHAR(255) NULL;
END

-- Adaugă coloana hourly_rate
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'hourly_rate')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [hourly_rate] NVARCHAR(50) NULL;
END

-- Adaugă coloana project_duration
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'project_duration')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [project_duration] NVARCHAR(100) NULL;
END

-- Adaugă coloana communication_style
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'communication_style')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [communication_style] NVARCHAR(255) NULL;
END

-- Adaugă coloana availability_status
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'availability_status')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [availability_status] NVARCHAR(50) NULL;
END

-- Adaugă coloana availability_hours
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'availability_hours')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [availability_hours] NVARCHAR(50) NULL;
END

-- Adaugă coloana rating
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'rating')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [rating] FLOAT NULL;
END

-- Adaugă coloana rating_count
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'rating_count')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [rating_count] INT NULL;
END

-- Adaugă coloana completed_projects
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'completed_projects')
BEGIN
    ALTER TABLE [dbo].[Users] ADD [completed_projects] INT NULL;
END

-- Actualizează utilizatorii existenți cu valori implicite
UPDATE [dbo].[Users]
SET 
    [headline] = 'Web Developer & UI/UX Designer',
    [about] = 'A passionate web developer and UI/UX designer with experience in creating responsive websites and user-friendly interfaces that deliver exceptional user experiences.',
    [skills] = 'HTML5,CSS3,JavaScript,React.js,Node.js,Figma,UI/UX Design,Responsive Design,API Integration,WordPress',
    [phone] = '+1 234 567 8900',
    [location] = 'New York, USA',
    [website] = 'www.portfolio.com',
    [preferred_project_types] = 'Web Development, UI/UX Design, E-commerce',
    [hourly_rate] = '$45 - $65 per hour',
    [project_duration] = 'Short to medium-term (1-6 months)',
    [communication_style] = 'Weekly video calls, daily messaging',
    [availability_status] = 'Available for work',
    [availability_hours] = '30 hrs/week',
    [rating] = 4.8,
    [rating_count] = 40,
    [completed_projects] = 23
WHERE 
    [headline] IS NULL; 