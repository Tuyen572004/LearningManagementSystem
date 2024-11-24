using System;
using System.Collections.Generic;
using LearningManagementSystem.EModels;
using Microsoft.EntityFrameworkCore;

namespace LearningManagementSystem.DataAccess;

public partial class LmsdbContext : DbContext
{
    public LmsdbContext()
    {
    }

    public LmsdbContext(DbContextOptions<LmsdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Cycle> Cycles { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Resourcecategory> Resourcecategories { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Submission> Submissions { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Teachersperclass> Teachersperclasses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=localhost;Database=lmsdb;User=root;Password=matkhaugitutim;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("assignments");

            entity.HasIndex(e => e.ClassId, "ClassId");

            entity.HasIndex(e => e.ResourceCategoryId, "ResourceCategoryId");

            entity.HasIndex(e => e.TeacherId, "TeacherId");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.DueDate).HasColumnType("date");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Class).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("assignments_ibfk_1");

            entity.HasOne(d => d.ResourceCategory).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ResourceCategoryId)
                .HasConstraintName("assignments_ibfk_3");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("assignments_ibfk_2");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("classes");

            entity.HasIndex(e => e.CourseId, "CourseId");

            entity.HasIndex(e => e.CycleId, "CycleId");

            entity.Property(e => e.ClassCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.ClassEndDate).HasColumnType("date");
            entity.Property(e => e.ClassStartDate).HasColumnType("date");

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classes_ibfk_1");

            entity.HasOne(d => d.Cycle).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classes_ibfk_2");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("courses");

            entity.HasIndex(e => e.DepartmentId, "DepartmentId");

            entity.Property(e => e.CourseCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.CourseDescription)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("courses_ibfk_1");
        });

        modelBuilder.Entity<Cycle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cycles");

            entity.Property(e => e.CycleCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.CycleDescription)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CycleEndDate).HasColumnType("date");
            entity.Property(e => e.CycleStartDate).HasColumnType("date");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("departments");

            entity.Property(e => e.DepartmentCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.DepartmentDesc)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("documents");

            entity.HasIndex(e => e.ClassId, "ClassId");

            entity.HasIndex(e => e.ResourceCategoryId, "ResourceCategoryId");

            entity.Property(e => e.DocumentName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.DocumentPath)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Class).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documents_ibfk_1");

            entity.HasOne(d => d.ResourceCategory).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ResourceCategoryId)
                .HasConstraintName("documents_ibfk_2");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("enrollments");

            entity.HasIndex(e => e.ClassId, "ClassId");

            entity.HasIndex(e => e.StudentId, "StudentId");

            entity.Property(e => e.EnrollmentDate).HasColumnType("date");

            entity.HasOne(d => d.Class).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("enrollments_ibfk_1");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("enrollments_ibfk_2");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.ClassId, "ClassId");

            entity.HasIndex(e => e.ResourceCategoryId, "ResourceCategoryId");

            entity.Property(e => e.NotificationText)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.PostDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Class).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notifications_ibfk_1");

            entity.HasOne(d => d.ResourceCategory).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ResourceCategoryId)
                .HasConstraintName("notifications_ibfk_2");
        });

        modelBuilder.Entity<Resourcecategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("resourcecategories");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Summary).HasColumnType("text");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("students");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNo).HasMaxLength(30);
            entity.Property(e => e.StudentCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.StudentName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("students_ibfk_1");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("submissions");

            entity.HasIndex(e => e.AssignmentId, "AssignmentId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.Grade).HasColumnType("double(5,2)");
            entity.Property(e => e.SubmissionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("submissions_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Submissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("submissions_ibfk_2");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("teachers");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PhoneNo).HasMaxLength(100);
            entity.Property(e => e.TeacherCode)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.TeacherName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teachers_ibfk_1");
        });

        modelBuilder.Entity<Teachersperclass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("teachersperclass");

            entity.HasIndex(e => e.TeacherId, "TeacherId");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Teachersperclasses)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("teachersperclass_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(5000);
            entity.Property(e => e.Role)
                .IsRequired()
                .HasColumnType("enum('student','teacher','admin')");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
