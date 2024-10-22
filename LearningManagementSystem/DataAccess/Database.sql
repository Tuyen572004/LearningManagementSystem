-- link to dbdiagram : https://dbdiagram.io/d/LMS-6717750097a66db9a3d71486


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

CREATE TABLE Courses (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CourseCode VARCHAR(10) NOT NULL,    -- Changed from CourseId to CourseCode
    CourseDescription VARCHAR(100) NOT NULL,
    DepartmentCode VARCHAR(10) NOT NULL, -- Changed from DepartmentId to DepartmentCode
    FOREIGN KEY (DepartmentCode) REFERENCES Departments(DepartmentCode)
);

CREATE TABLE Departments (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    DepartmentCode VARCHAR(10) NOT NULL, -- Changed from DepartmentId to DepartmentCode
    DepartmentDesc VARCHAR(100) NOT NULL
);

CREATE TABLE Cycles (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CycleCode VARCHAR(10) NOT NULL,     -- Changed from CycleId to CycleCode
    CycleDescription VARCHAR(100) NOT NULL,
    CycleStartDate DATE NOT NULL,
    CycleEndDate DATE NOT NULL
);

-- Inserting example data into the Cycles table
-- INSERT INTO Cycles (CycleCode, CycleDescription, CycleStartDate, CycleEndDate) VALUES
-- ('C1', 'Semester 2023 - 1st', '2023-09-01', '2023-12-15'),
-- ('C2', 'Semester 2024 - 2nd', '2024-01-10', '2024-05-15'),
-- ('C3', 'Semester 2024 - 3rd', '2024-06-01', '2024-08-15');

CREATE TABLE Enrollments (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CourseCode VARCHAR(10),
    CycleCode VARCHAR(10),
    StudentCode VARCHAR(10),
    EnrollmentDate DATE NOT NULL,
    Cancelled BOOLEAN NOT NULL,
    CancellationReason VARCHAR(100),
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode),
    FOREIGN KEY (CycleCode) REFERENCES Cycles(CycleCode),
    FOREIGN KEY (StudentCode) REFERENCES Students(StudentCode)
);

CREATE TABLE Classes (
    Id INT AUTO_INCREMENT PRIMARY KEY,  -- New auto-incrementing Id field
    CourseCode VARCHAR(10),
    CycleCode VARCHAR(10),
    ClassStartDate DATE NOT NULL,
    ClassEndDate DATE NOT NULL,
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode),
    FOREIGN KEY (CycleCode) REFERENCES Cycles(CycleCode)
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
    CourseCode VARCHAR(10),
    CycleCode VARCHAR(10),
    TeacherCode VARCHAR(10),
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode),
    FOREIGN KEY (CycleCode) REFERENCES Cycles(CycleCode),
    FOREIGN KEY (TeacherCode) REFERENCES Teachers(TeacherCode)
    -- Note: This table associates teachers with specific classes
);

-- New table for Assignments
CREATE TABLE Assignments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CourseCode VARCHAR(10),
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    DueDate DATE NOT NULL,
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode)
);

-- New table for Notifications
CREATE TABLE Notifications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CourseCode VARCHAR(10),
    TeacherId INT,
    NotificationText TEXT NOT NULL,
    PostDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode),
    FOREIGN KEY (TeacherId) REFERENCES Teachers(Id)
);

-- New table for Documents
CREATE TABLE Documents (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CourseCode VARCHAR(10),
    DocumentTitle VARCHAR(100) NOT NULL,
    DocumentPath VARCHAR(255) NOT NULL,
    UploadDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CourseCode) REFERENCES Courses(CourseCode)
);

-- New table for Submissions
CREATE TABLE Submissions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AssignmentId INT,
    StudentCode VARCHAR(10),
    SubmissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Answer TEXT NOT NULL,
    FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
    FOREIGN KEY (StudentCode) REFERENCES Students(StudentCode)
);

CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Role ENUM('student', 'teacher', 'admin') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);