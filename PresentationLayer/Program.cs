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
using Microsoft.AspNetCore.DataProtection;
using BusinessLogicLayer.User_Management.Interfaces;
using BusinessLogicLayer.User_Management.Services;
using BusinessLogicLayer.Questions_Mangment.Services;
using PresentationLayer.Hubs;
using StackExchange.Redis;
using BusinessLogicLayer.RAG.Interfaces;
using BusinessLogicLayer.RAG.Services;
//using PresentationLayer.Hubs;
//using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Redis connection BEFORE ConfigureServices

// Configure services   
ConfigureServices(builder.Services, builder.Configuration);

// ✅ Add SignalR services BEFORE builder.Build()
builder.Services.AddSignalR();

// Configure authentication and authorization
ConfigureAuthentication(builder.Services);
ConfigureAuthorization(builder.Services);

// ✅ Build the app AFTER all services are added
var app = builder.Build();

// Map SignalR hub
app.MapHub<AdminNotificationHub>("/adminHub");

// Configure middleware (error handling, routing, etc.)
ConfigureMiddleware(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Use session for server-side storage
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Use session-backed TempData (instead of cookie-backed)
    services.AddControllersWithViews()
            .AddSessionStateTempDataProvider();

    services.AddRazorPages();

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Users\ABD HAMED\Desktop\vs\exam-man - Copy\Keys"))
    .SetApplicationName("ExamSystem");

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
    
    services.AddScoped<IQuestionParser, QuestionParser>();
    services.AddScoped<ISignupNotificationRepository, SignupNotificationRepository>();
    services.AddScoped<ISignupNotificationService, SignupNotificationService>();
    services.AddScoped<IResetTokenRepository, ResetTokenRepository>();
    services.AddScoped<IConfirmationCodeService, ConfirmationCodeService>();
    services.AddScoped<IConfirmationCodeRepository, ConfirmationCodeRepository>();
    services.AddScoped<ILectureFileRepository, LectureFileRepository>();
    services.AddScoped<ILectureFileService, LectureFileService>();
    services.AddHttpClient<IRagService, RagService>(client =>
    {
        client.Timeout = Timeout.InfiniteTimeSpan;
    });






    builder.Services.AddTransient<IEmailService, EmailService>();
   
  //  services.AddHttpClient<IOpenAIService, CohereService>();




    //builder.Services.AddSingleton<IConfirmationCodeService, ConfirmationCodeService>(); // Or scoped with EF Core




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
    app.UseSession();
    app.UseAuthentication(); // Add authentication middleware
    app.UseAuthorization(); // Add authorization middleware
                            // Configure Rotativa
    RotativaConfiguration.Setup(app.Environment.WebRootPath);

    // Configure the routes
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=index}/{id?}");

    // If using Razor Pages
    app.MapRazorPages();

}
