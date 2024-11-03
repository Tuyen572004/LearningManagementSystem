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
    EnrollmentYear INT NOT NULL,        -- This field is added as the graduation field is added.
                                        -- Consider adding "LearningState" field, to control expelled students (that would not graduate of course)
    GraduationYear INT DEFAULT NULL,    -- This field is added for the classification purpose, as students graduate overtime.
    Email VARCHAR(100) NOT NULL,
    BirthDate DATE NOT NULL,
    PhoneNo VARCHAR(30),
    -- UserId INT NOT NULL,  -- Foreign key to Users table
    UserId INT DEFAULT NULL,
        -- Removed NOT NULL, as you would naturally add students first
        -- then you would add the corresponding number of users later.
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

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


-- Create ResourceCategory table
CREATE TABLE ResourceCategories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Summary TEXT
);

-- Create Assignments table with ResourceCategoryId
CREATE TABLE Assignments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,               -- Foreign key to Classes table
    TeacherId INT NOT NULL,             -- Foreign key to Teachers table
    ResourceCategoryId INT,             -- Foreign key to ResourceCategory table
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    DueDate DATE NOT NULL,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id),
    FOREIGN KEY (TeacherId) REFERENCES Teachers(Id),
    FOREIGN KEY (ResourceCategoryId) REFERENCES ResourceCategories(Id)
);

-- Create Notifications table with ResourceCategoryId
CREATE TABLE Notifications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,               -- Foreign key to Classes table
    ResourceCategoryId INT,             -- Foreign key to ResourceCategory table
    Title VARCHAR(100) NOT NULL,
    NotificationText TEXT NOT NULL,
    PostDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id),
    FOREIGN KEY (ResourceCategoryId) REFERENCES ResourceCategories(Id)
);

-- Create Documents table with ResourceCategoryId
CREATE TABLE Documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClassId INT NOT NULL,               -- Foreign key to Classes table
    ResourceCategoryId INT,             -- Foreign key to ResourceCategory table
    Title VARCHAR(100) NOT NULL,
    DocumentName VARCHAR(100) NOT NULL,
    DocumentPath VARCHAR(255) NOT NULL,
    UploadDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassId) REFERENCES Classes(Id),
    FOREIGN KEY (ResourceCategoryId) REFERENCES ResourceCategories(Id)
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

INSERT INTO Users (Id, Username, PasswordHash, Email, Role, CreatedAt) VALUES
(1, 'student', 'YWRtaW5zdHVkZW50', 'email', 'student', '1000-01-01 00:00:00'),
(2, 'teacher', 'YWRtaW50ZWFjaGVy', 'email1', 'teacher', '1000-01-01 00:00:00');


-- INSERT INTO Cycles
INSERT INTO Cycles (Id, CycleCode, CycleDescription, CycleStartDate, CycleEndDate) VALUES
(1, '2023FALL', 'Fall 2023 Semester', '2023-09-01', '2023-12-15'),
(2, '2024SPRING', 'Spring 2024 Semester', '2024-01-10', '2024-05-20');

INSERT INTO Departments (Id, DepartmentCode, DepartmentDesc) VALUES
(1, 'CS', 'Computer Science'),
(2, 'SE', 'Software Engineer');

INSERT INTO Courses (Id, CourseCode, CourseDescription, DepartmentId) VALUES
(1, 'CSE101', 'Introduction to Computer Science', 1),
(2, 'CSE102', 'Data Structures and Algorithm', 1),
(3, 'CSE103', 'Operating Systems', 2),
(4, 'CSC104', 'Computer Networks', 1),
(5, 'CSE105', 'Database Management Systems', 1);


-- INSERT INTO Classes
INSERT INTO Classes (Id, ClassCode, CourseId, CycleId, ClassStartDate, ClassEndDate) VALUES
(1, 'CS101-01', 1, 1, '2023-09-01', '2023-12-15'),
(2, 'MATH101-01', 2, 1, '2023-09-01', '2023-12-15'),
(3, 'ENG101-01', 3, 1, '2023-09-01', '2023-12-15');

-- INSERT INTO Teachers
INSERT INTO Teachers (Id, TeacherCode, TeacherName, Email, PhoneNo, UserId) VALUES
(1, 'T001', 'John Doe', 'johndoe@example.com', '123-456-7890', 1);
-- (2, 'T002', 'Jane Smith', 'janesmith@example.com', '098-765-4321', 2),
-- (3, 'T003', 'Emily Johnson', 'emilyjohnson@example.com', '555-555-5555', 3);


-- INSERT INTO Students
INSERT INTO Students (Id, StudentCode, StudentName, EnrollmentYear, GraduationYear, Email, BirthDate, PhoneNo, UserId) VALUES
(1, 'S001', 'Alice Johnson', 2021, null, 'alice.johnson@example.com', '2003-05-15', '555-1234', 1),
(2, 'S002', 'Bob Smith', 2020, 2024, 'bob.smith@example.com', '2002-08-22', '555-5678', 2),
(3, 'S003', 'Charlie Brown', 2022, null, 'charlie.brown@example.com', '2004-11-30', '555-8765', 3);

-- INSERT INTO Students
INSERT INTO Students (Id, StudentCode, StudentName, EnrollmentYear, GraduationYear, Email, BirthDate, PhoneNo, UserId) VALUES
(1, 'S001', 'Alice Johnson', 2021, null, 'alice.johnson@example.com', '2003-05-15', '555-1234', 1),
(2, 'S002', 'Bob Smith', 2020, 2024, 'bob.smith@example.com', '2002-08-22', '555-5678', null),
(3, 'S003', 'Charlie Brown', 2022, null, 'charlie.brown@example.com', '2004-11-30', '555-8765', null),
(4, 'S004', 'David Wilson', 2021, null, 'david.wilson@example.com', '2003-07-20', '555-4321', null),
(5, 'S005', 'Eva Green', 2020, null, 'eva.green@example.com', '2002-09-15', '555-8765', null),
(6, 'S006', 'Frank White', 2022, null, 'frank.white@example.com', '2004-12-01', '555-5678', null),
(7, 'S007', 'Grace Black', 2021, null, 'grace.black@example.com', '2003-03-25', '555-1234', null),
(8, 'S008', 'Hannah Blue', 2020, 2024, 'hannah.blue@example.com', '2002-11-30', '555-4321', null);

-- INSERT INTO Enrollments
INSERT INTO Enrollments (Id, ClassId, StudentId, EnrollmentDate) VALUES

-- INSERT INTO Enrollments
INSERT INTO Enrollments (Id, ClassId, StudentId, EnrollmentDate) VALUES
(1, 1, 1, '2023-09-01'),
(3, 1, 3, '2023-09-01'),
(4, 2, 4, '2023-09-01'),
(5, 2, 5, '2023-09-01'),
(6, 2, 6, '2023-09-01'),
(7, 3, 7, '2023-09-01'),

-- INSERT INTO ResourceCategories
INSERT INTO ResourceCategories (Id, Name,Summary) VALUES
(1, 'Homeworks', 'Assignments to be completed by students.'),
(2, 'Lecture Notes', 'Notes from class lectures.'),
(3, 'Notifications', 'Informations about upcoming events');

-- INSERT INTO Notifications
INSERT INTO Notifications (Id, ClassId, ResourceCategoryId, NotificationText,Title) VALUES
(1, 1, 1, 'Homework 1 is due next week.','Noti 1'),
(2, 1, 2, 'Lecture notes for week 1 are available.','Noti 2'),
(3, 2, 3, 'Exam 1 will be held next month.', 'Noti 3');

-- INSERT INTO Assignments
INSERT INTO Assignments (Id, ClassId, TeacherId, ResourceCategoryId, Title, Description, DueDate) VALUES
(1, 1, 1, 1, 'Homework 1', 'Complete the exercises in chapter 1.', '2023-11-01'),
(2, 1, 1, 1, 'Homework 2', 'Complete the exercises in chapter 2.', '2023-11-08'),
(3, 2, 1, 3, 'Midterm Exam', 'Study chapters 1-5.', '2023-12-01');

-- INSERT INTO Documents
INSERT INTO Documents (Id, ClassId, ResourceCategoryId,Title, DocumentName, DocumentPath) VALUES
(1, 1, 2,'Week 1 Lecture Notes','Intro To DSA', '/path/to/lecture1.pdf'),
(2, 1, 2, 'Week 2 Lecture Notes','300 Bai Code Thieu Nhi', '/path/to/lecture2.pdf'),
(3, 2, 3, 'Midterm Exam Study Guide','Cach de ban 1 ty goi me', '/path/to/studyguide.pdf');
