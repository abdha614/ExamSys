using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataAccessLayer.Models;
using Rotativa.AspNetCore;
using DocumentFormat.OpenXml.Spreadsheet;
using BusinessLogicLayer.Questions_Mangment.Interfaces;
//using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Configure services   
ConfigureServices(builder.Services, builder.Configuration);

// Configure authentication and authorization
ConfigureAuthentication(builder.Services);
ConfigureAuthorization(builder.Services);

var app = builder.Build();

// Configure middleware (error handling, routing, etc.)
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllersWithViews();
    services.AddRazorPages(); // Register Razor Pages services.

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Add AutoMapper
    services.AddAutoMapper(typeof(BusinessLogicLayer.Mapping.MappingProfile), typeof(PresentationLayer.Mapping.MappingProfile), typeof(DataAccessLayer.Mapping.MappingProfile));

    // Register Repositories
    services.AddScoped<IRoleRepository, RoleRepository>();
    services.AddScoped<IUserRepository, UserRepository>();
   

    // Register Business Logic Services
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IRoleService, RoleService>();
    services.AddScoped<IHistoryRepository, HistoryRepository>();
    services.AddScoped<IHistoryService, HistoryService>();
    services.AddScoped<IQuestionService, QuestionService>();
    services.AddScoped<IQuestionRepository, QuestionRepository>();
    services.AddScoped<IAnswerRepository, AnswerRepository>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<ICourseRepository, CourseRepository>();
    services.AddScoped<ICourseService, CourseService>();
    services.AddScoped<IQuestionTypeService, QuestionTypeService>();
    services.AddScoped<IQuestionTypeRepository, QuestionTypeRepository>();
    services.AddScoped<IQuestionImportService, CsvImportService>();
    services.AddScoped<IQuestionImportService, DocxImportService>();
    services.AddScoped<IDifficultyLevelRepository, DifficultyLevelRepository>();
    services.AddScoped<ILectureRepository, LectureRepository>();
    services.AddScoped<IExamRepository, ExamRepository>();
    services.AddScoped<IExamService, ExamService>();

    // Register PasswordHasher
    services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    services.AddScoped<IDifficultyLevelService, DifficultyLevelService>();
}

void ConfigureAuthentication(IServiceCollection services)
{
    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = ".AspNetCore.Cookies";
            options.Cookie.HttpOnly = true; // Protect against JavaScript access
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS
            options.Cookie.SameSite = SameSiteMode.Lax; // Default to Lax for CSRF protection
            options.SlidingExpiration = false; // Do not extend cookie expiration on activity
            options.Cookie.Expiration = null; // Ensures the cookie is session-based
            options.LoginPath = "/Auth/Login"; // Redirect to login page when unauthorized
            options.LogoutPath = "/Auth/Logout"; // Redirect after logging out
            options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect if access is denied
        });
}

void ConfigureAuthorization(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ProfessorPolicy", policy => policy.RequireRole("Professor"));
        options.AddPolicy("StudentPolicy", policy => policy.RequireRole("Student"));
    });
}

void ConfigureMiddleware(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts(); // Adds HTTP Strict Transport Security (HSTS)
    }
    else
    {
        app.UseDeveloperExceptionPage(); // Only in development
    }

    app.UseHttpsRedirection(); // Enforce HTTPS
    app.UseStaticFiles(); // Serve static files
    app.UseRotativa(); // after app.UseStaticFiles();
    app.UseRouting(); // Adds routing
    app.UseAuthentication(); // Add authentication middleware
    app.UseAuthorization(); // Add authorization middleware
                            // Configure Rotativa
    RotativaConfiguration.Setup(app.Environment.WebRootPath);

    // Configure the routes
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=login}/{id?}");

    // If using Razor Pages
    app.MapRazorPages();
}
