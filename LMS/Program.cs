using LMS.EntityСontext;
using LMS.Services.AuthService;
using LMS.Services;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Numerics;
using ASTU_LMS.StartupPrep;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterApplicationServices(builder.Configuration);

var app = builder.Build();

app.ConfigureMiddleware();
app.RegisterEndpoints();

SetupDatabase();
CreateNeedDirectory();

app.Run();

void CreateNeedDirectory()
{
    var necessaryDirectories = new[] { "Repositories", "RepositoriesTemplates" };
    foreach (var item in necessaryDirectories)
    {
        if (!Directory.Exists(item))
        {
            Directory.CreateDirectory(item);
        }
    }
}

void SetupDatabase()
{
    ApplicationContext context = new ApplicationContext();
    // check and add roles
    AuthService auth = new AuthService(builder.Configuration);
    //context.Database.EnsureCreated();

    if (!context.Groups.Any())
    {
        context.Groups.Add(new Group { Name = "ДИПР-31", Year = 2019 });
        context.Groups.Add(new Group
        { Name = "ДИНР-31", Year = 2019 });
        context.SaveChanges();
    }

    if (!context.Roles.Any())
    {
        context.Roles.Add(new Role { RoleId = 1, RoleName = RoleEnum.Student });
        context.Roles.Add(new Role { RoleId = 2, RoleName = RoleEnum.Teacher });
        context.Roles.Add(new Role { RoleId = 3, RoleName = RoleEnum.Admin });

        context.SaveChanges();
    }

    if (!context.Users.Any())
    {
        var student = new User { Surname = "Михайлов", Name = "Дмитрий", Patronymic = "Владимирович", Email = "tmpstudent@mail.com", PwHash = auth.GetHashedPassword("tmpstudent"), GroupId = 1, GitUsername = "tmpstudent" };
        var student1 = new User { Surname = "Иванов", Name = "Иван", Patronymic = "Иванович", Email = "ivan@mail.com", PwHash = auth.GetHashedPassword("ivan"), GroupId = 2, GitUsername = "ivan" };
        var student2 = new User { Surname = "Петров", Name = "Петр", Patronymic = "Петрович", Email = "petr@mail.com", PwHash = auth.GetHashedPassword("petr"), GroupId = 1, GitUsername = "petr" };
        var student3 = new User { Surname = "Макаров", Name = "Макар", Patronymic = "Макарович", Email = "makar@mail.com", PwHash = auth.GetHashedPassword("makar"), GroupId = 2, GitUsername = "makar" };
        var student4 = new User { Surname = "Сидорова", Name = "Александра", Patronymic = "Михайловна", Email = "alexandrochka@mail.com", PwHash = auth.GetHashedPassword("alexandrochka"), GroupId = 1, GitUsername = "alexandrochka" };
        var student5 = new User { Surname = "Владов", Name = "Владислав", Patronymic = "Владиславович", Email = "vlad@mail.com", PwHash = auth.GetHashedPassword("vlad"), GroupId = 2, GitUsername = "vlad" };
        var student6 = new User { Surname = "Федоров", Name = "Федор", Patronymic = "Федорович", Email = "fedor@mail.com", PwHash = auth.GetHashedPassword("fedor"), GroupId = 1, GitUsername = "fedor" };
        var teacher = new User { Surname = "Семёнов", Name = "Иван", Patronymic = "Васильевич", Email = "tmpprepod@mail.com", PwHash = auth.GetHashedPassword("tmpprepod"), GitUsername = "tmpprepod" };
        var admin = new User { Surname = "Админов", Name = "Админ", Patronymic = "Админович", Email = "tmpadmin@mail.com", PwHash = auth.GetHashedPassword("tmpadmin"), GitUsername = "tmpadmin" };

        context.Users.Add(student);
        context.Users.Add(student1);
        context.Users.Add(student2);
        context.Users.Add(student3);
        context.Users.Add(student4);
        context.Users.Add(student5);
        context.Users.Add(student6);
        context.Users.Add(admin);
        context.Users.Add(teacher);
        context.SaveChanges();

        context.UserRoles.Add(new UserRole { UserId = student.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student1.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student2.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student3.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student4.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student5.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = student6.Id, RoleId = 1 });
        context.UserRoles.Add(new UserRole { UserId = teacher.Id, RoleId = 2 });
        context.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = 3 });

        context.SaveChanges();
    }
    if (!context.Courses.Any())
    {
        context.Courses.Add(new Course { Name = "Программирование", UserId = 9 });
        context.Courses.Add(new Course { Name = "Программирование2", UserId = 9 });
        context.Courses.Add(new Course { Name = "Программирование3", UserId = 9 });
        context.Courses.Add(new Course { Name = "Программирование4", UserId = 9 });
        context.Courses.Add(new Course { Name = "Программирование5", UserId = 9 });
        context.SaveChanges();

        var student3 = context.Users.FirstOrDefault(t => t.Id == 4);

        var Test = context.Courses.First();
        Test.Users.Add(student3);
        context.SaveChanges();
    }
}