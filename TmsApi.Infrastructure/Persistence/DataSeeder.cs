using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Persistence;
public static class DataSeeder
{
    private static readonly (string Code, string Title, int MaxCapacity) [] Courses = [
        ("CSE-101", "Web Development Fundamentals", 30),
        ("CSE-102", "TypeScript Essentials",30),
        ("CSE-103", "Git and Collaborative Workflows", 25),
        ("CSE-201", "ASP.NET Core Fundamentals",28),
        ("CSE-202", "Entity Framework Core and PostgreSQL", 28),
        ("CSE-203", "Building RESTful Web APIs",28),
        ("CSE-301", "Advanced Web API Patterns",24),
        ("CSE-302", "Angular Fundamentals",26),
        ("CSE-303", "Angular Advanced",24),
        ("CSE-304", "Full-Stack Integration",22),
        ("CSE-305", "Testing and Quality Assurance", 22),
        ("CSE-306", "Security and Authentication", 20),
        ("DAT-101", "Database Design Foundations", 30),
        ("DAT-201", "Advanced SQL and Indexing",26),
        ("DAT-202", "Data Modelling for the Web",26),
        ("ARC-101", "Software Architecture Patterns", 22),
        ("ARC-201", "Cloud-Native Architecture",22),
        ("DEV-101", "DevOps Foundations",24),
        ("DEV-201", "Continuous Delivery Pipelines", 22),
        ("MOB-101", "Mobile App Foundations",24),
        ("MOB-201", "Cross-Platform Mobile",22),
        ("AI-101", "Applied Machine Learning",22),
        ("AI-201", "Generative AI for Developers", 18),
        ("UX-101", "UX Research and Wireframing", 24),
        ("UX-201", "Design Systems and Tokens",22),
        ];
        
    private static readonly (string RegistrationNumber, string Name, decimal GPA, bool IsActive) [] Students = [
            ("REG-001", "Alice Johnson", 3.8m, true),
            ("REG-002", "Bob Smith", 3.5m, true),
            ("REG-003", "Charlie Brown", 3.2m, false),
            ("REG-004", "Diana Prince", 3.9m, true),
            ("REG-005", "Ethan Hunt", 3.6m, true),
            ("REG-006", "Fiona Gallagher", 3.4m, false),
            ("REG-007", "George Martin", 3.7m, true),
            ("REG-008", "Hannah Baker", 3.1m, false),
            ("REG-009", "Ian Malcolm", 3.5m, true),
            ("REG-010", "Julia Roberts", 3.8m, true),
            ("REG-011", "Kevin Hart", 3.2m, false),
            ("REG-012", "Laura Palmer", 3.6m, true),
            ("REG-013", "Michael Scott", 3.4m, false),
            ("REG-014", "Nancy Drew", 3.9m, true),
            ("REG-015", "Oscar Wilde", 3.7m, true),
            ("REG-016", "Pam Beesly", 3.5m, false),
            ("REG-017", "Quentin Tarantino", 3.8m, true),
            ("REG-018", "Rachel Green", 3.1m, false),
            ("REG-019", "Steve Rogers", 3.6m, true),
            ("REG-020", "Tina Fey", 3.4m, false),
            ("REG-021", "Uma Thurman", 3.9m, true),
            ("REG-022", "Victor Hugo", 3.7m, true),
            ("REG-023", "Wendy Darling", 3.5m, false),
            ("REG-024", "Xander Harris", 3.8m, true),
            ("REG-025", "Yara Greyjoy", 3.2m, false),
            ("REG-026", "Zoe Saldana", 3.6m, true)
        ];

    public static async Task SeedCourseAsync(TmsDbContext context, CancellationToken ct = default)
    {
        await context.Database.MigrateAsync(ct);

        if (await context.Courses.AnyAsync(ct))
        {
            Console.WriteLine("\t\t_------------------------------------------------------------_");
            Console.WriteLine("\t\t Courses Table already seeded. Skipping seeding process.");
            Console.WriteLine("\t\t_------------------------------------------------------------_");
            return;
        }
        foreach (var (code, title, maxCapacity) in Courses)
        {
            context.Courses.Add(new Course
            {
            Code = code,
            Title = title,
            MaxCapacity = maxCapacity
            });
        }

        await context.SaveChangesAsync(ct);
    }
    public static async Task SeedStudentAsync(TmsDbContext context, CancellationToken ct = default)
    {
        await context.Database.MigrateAsync(ct);

        if (await context.Students.AnyAsync(ct))
        {
            Console.WriteLine("\t\t_------------------------------------------------------------_");
            Console.WriteLine("\t\tStudents Table already seeded. Skipping seeding process.");
            Console.WriteLine("\t\t_------------------------------------------------------------_");
            return;
        }

        foreach (var (registrationNumber, name, gpa, isActive) in Students)
        {
            context.Students.Add(new Student
            {
                RegistrationNumber = registrationNumber,
                Name = name,
                GPA = gpa,
                IsActived = isActive
            });
        }

        await context.SaveChangesAsync(ct);
    }
}