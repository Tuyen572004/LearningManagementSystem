-- link to dbdiagram : https://dbdiagram.io/d/LMS-6717750097a66db9a3d71486

drop database if exists LMSdb;

create database LMSDb;

use LMSDb;

CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(5000) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Role ENUM('student', 'teacher', 'admin') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE Students (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    StudentCode VARCHAR(10) NOT NULL,   -- Changed from StudentId to StudentCode
    StudentName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    BirthDate DATE NOT NULL,
    PhoneNo VARCHAR(30),
    UserId INT NOT NULL,  -- Foreign key to Users table
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- CREATE TABLE Students (
--     Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
--     StudentCode VARCHAR(10) NOT NULL,   -- Changed from StudentId to StudentCode
--     StudentName VARCHAR(100) NOT NULL,
--     EnrollmentYear INT NOT NULL,
--     GraduationYear INT,
--     Email VARCHAR(100) NOT NULL,
--     BirthDate DATE NOT NULL,
--     PhoneNo VARCHAR(30),
--     UserId INT,  -- Foreign key to Users table
--     -- Removing NOT NULL constraint so that admin can import students directly
--     -- without the need to create a user account immediately 
--     FOREIGN KEY (UserId) REFERENCES Users(Id)
-- );

CREATE TABLE Departments (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    DepartmentCode VARCHAR(10) NOT NULL, -- Changed from DepartmentId to DepartmentCode
    DepartmentDesc VARCHAR(100) NOT NULL
);

CREATE TABLE Courses (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CourseCode VARCHAR(10) NOT NULL,    -- Changed from CourseId to CourseCode
    CourseDescription VARCHAR(100) NOT NULL,
    DepartmentId INT NOT NULL,          -- Foreign key to Departments table
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);


CREATE TABLE Cycles (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CycleCode VARCHAR(10) NOT NULL,     -- Changed from CycleId to CycleCode
    CycleDescription VARCHAR(100) NOT NULL,
    CycleStartDate DATE NOT NULL,
    CycleEndDate DATE NOT NULL
);

CREATE TABLE Classes (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    ClassCode VARCHAR(10) NOT NULL,     -- Changed from ClassId to ClassCode
    CourseId INT NOT NULL,              -- Foreign key to Courses table
    CycleId INT NOT NULL,               -- Foreign key to Cycles table
    ClassStartDate DATE NOT NULL,
    ClassEndDate DATE NOT NULL,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id),
    FOREIGN KEY (CycleId) REFERENCES Cycles(Id)
);

CREATE TABLE Enrollments (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    ClassId INT NOT NULL,                -- Foreign key to Classes table
    StudentId INT NOT NULL,              -- Foreign key to Students table
    EnrollmentDate DATE NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id),
    FOREIGN KEY (StudentId) REFERENCES Students(Id)
);

-- CREATE TABLE Enrollments (
--     Id INT AUTO_INCREMENT PRIMARY KEY,
--     StudentId INT NOT NULL, -- Foreign key to Students table
--     ClassId INT NOT NULL, -- Foreign key to Classes table
--     EnrollmentState ENUM('Enrolled', 'Dropped', 'Failed', 'Completed') NOT NULL DEFAULT 'Enrolled',
--     CreatedAt DATE NOT NULL,
--     Description VARCHAR(100),
--     FOREIGN KEY (StudentId) REFERENCES Students(Id),
--     FOREIGN KEY (ClassId) REFERENCES Classes(Id)
-- );


CREATE TABLE Teachers (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    TeacherCode VARCHAR(10) NOT NULL,   -- Changed from TeacherId to TeacherCode
    TeacherName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PhoneNo VARCHAR(100),
    UserId INT NOT NULL,  -- Foreign key to Users table
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE TeachersPerClass (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    ClassId INT NOT NULL,              -- Foreign key to Courses table
    TeacherId INT NOT NULL,             -- Foreign key to Teachers table
    
    FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
);

CREATE TABLE Assignments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,               -- Foreign key to Classes table
    TeacherId INT NOT NULL,             -- Foreign key to Teachers table
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    DueDate DATE NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id),
    FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
);

CREATE TABLE Notifications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,              -- Foreign key to Courses table
    NotificationText TEXT NOT NULL,
    PostDate DATETIME DEFAULT CURRENT_TIMESTAMP
     
    
);

CREATE TABLE Documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,              -- Foreign key to Courses table
    DocumentTitle VARCHAR(100) NOT NULL,
    DocumentPath VARCHAR(255) NOT NULL,
    UploadDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id)
);

CREATE TABLE Submissions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AssignmentId INT NOT NULL,          -- Foreign key to Assignments table
    StudentId INT NOT NULL,             -- Foreign key to Students table
    SubmissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Answer TEXT NOT NULL,
    FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
    FOREIGN KEY (StudentId) REFERENCES Students(Id)
);

