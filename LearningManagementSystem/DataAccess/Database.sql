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

CREATE TABLE ResourceCategory (
    id INT PRIMARY KEY AUTO_INCREMENT,
    type VARCHAR(50) NOT NULL,
    summary TEXT
);

CREATE TABLE Announcement (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ClassID INT NOT NULL,
    ResourceCategoryID INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    DatePosted DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassID) REFERENCES Classes(Id),
    FOREIGN KEY (ResourceCategoryID) REFERENCES ResourceCategory(id)
);

CREATE TABLE Material (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ClassID INT NOT NULL,
    ResourceCategoryID INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    URL VARCHAR(255),
    DatePosted DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassID) REFERENCES Classes(Id),
    FOREIGN KEY (ResourceCategoryID) REFERENCES ResourceCategory(id)
);

CREATE TABLE Assignment (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ClassID INT NOT NULL,
    ResourceCategoryID INT NOT NULL,
    Title VARCHAR(255) NOT NULL,
    URL VARCHAR(255),
    DatePosted DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClassID) REFERENCES Classes(Id),
    FOREIGN KEY (ResourceCategoryID) REFERENCES ResourceCategory(id)
);
-- Insert sample data into Users table
-- INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES
-- ('student', 'hashedpassword1', 'student1@example.com', 'student'),
-- ('teacher', 'hashedpassword2', 'teacher1@example.com', 'teacher'),
-- ('admin1', 'hashedpassword3', 'admin1@example.com', 'admin');

-- Insert sample data into Students table
-- INSERT INTO Students (StudentCode, StudentName, Email, BirthDate, PhoneNo, UserId) VALUES
-- ('S001', 'John Doe', 'john.doe@example.com', '2000-01-01', '1234567890', 1);

-- Insert sample data into Departments table
INSERT INTO Departments (DepartmentCode, DepartmentDesc) VALUES
('D001', 'Computer Science'),
('D002', 'Mathematics');

-- Insert sample data into Courses table
INSERT INTO Courses (CourseCode, CourseDescription, DepartmentId) VALUES
('C001', 'Introduction to Programming', 1),
('C002', 'Calculus I', 2);

-- Insert sample data into Cycles table
INSERT INTO Cycles (CycleCode, CycleDescription, CycleStartDate, CycleEndDate) VALUES
('CY001', 'Fall 2023', '2023-09-01', '2023-12-15');

-- Insert sample data into Classes table
INSERT INTO Classes (ClassCode, CourseId, CycleId, ClassStartDate, ClassEndDate) VALUES
('CL001', 1, 1, '2023-09-01', '2023-12-15');

-- Insert sample data into Enrollments table
INSERT INTO Enrollments (ClassId, StudentId, EnrollmentDate) VALUES
(1, 1, '2023-09-01');

-- Insert sample data into Teachers table
INSERT INTO Teachers (TeacherCode, TeacherName, Email, PhoneNo, UserId) VALUES
('T001', 'Jane Smith', 'jane.smith@example.com', '0987654321', 2);

-- Insert sample data into TeachersPerClass table
INSERT INTO TeachersPerClass (ClassId, TeacherId) VALUES
(1, 1);

-- Insert sample data into Assignments table
INSERT INTO Assignments (ClassId, TeacherId, Title, Description, DueDate) VALUES
(1, 1, 'Assignment 1', 'Description for Assignment 1', '2023-10-01');

-- Insert sample data into Notifications table
INSERT INTO Notifications (ClassId, NotificationText) VALUES
(1, 'Welcome to the class!');

-- Insert sample data into Documents table
INSERT INTO Documents (ClassId, DocumentTitle, DocumentPath) VALUES
(1, 'Syllabus', '/path/to/syllabus.pdf');

-- Insert sample data into Submissions table
INSERT INTO Submissions (AssignmentId, StudentId, Answer) VALUES
(1, 1, 'Answer to Assignment 1');

-- Insert sample data into ResourceCategory table
INSERT INTO ResourceCategory (type, summary) VALUES
('Announcement', 'General announcements'),
('Material', 'Course materials'),
('Assignment', 'Assignments');

-- Insert sample data into Announcement table
INSERT INTO Announcement (ClassID, ResourceCategoryID, Title, Description) VALUES
(1, 1, 'First Announcement', 'This is the first announcement.');

-- Insert sample data into Material table
INSERT INTO Material (ClassID, ResourceCategoryID, Title, Description, URL) VALUES
(1, 2, 'Lecture Notes', 'Notes for the first lecture', 'http://example.com/lecture1');

-- Insert sample data into Assignment table
INSERT INTO Assignment (ClassID, ResourceCategoryID, Title, Description, URL) VALUES
(1, 3, 'Homework 1', 'First homework assignment', 'http://example.com/homework1');