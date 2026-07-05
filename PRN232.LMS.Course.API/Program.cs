using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using PRN232.LMS.Course.API.Infrastructure.Data;
using PRN232.LMS.Course.API.Infrastructure.Repositories;
using PRN232.LMS.Course.API.Application.Services;
using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Protos;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/course-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Input token only.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "a-very-long-and-secure-secret-key-123456789"))
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DBDefault");
builder.Services.AddDbContext<CourseDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IGenericRepository<Course>, GenericRepository<Course>>();
builder.Services.AddScoped<IGenericRepository<Enrollment>, GenericRepository<Enrollment>>();
builder.Services.AddScoped<IGenericRepository<Grade>, GenericRepository<Grade>>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddMvc();

builder.Services.AddGrpcClient<StudentGrpc.StudentGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcSettings:StudentServiceUrl"] ?? "http://localhost:5002");
});

var app = builder.Build();

// Auto apply migrations and seeding on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    context.Database.Migrate();

    DbSeeder.Seed(context);
}

app.UseMiddleware<PRN232.LMS.Course.API.Middlewares.ExceptionMiddleware>();
app.UseMiddleware<PRN232.LMS.Course.API.Middlewares.ExceptionMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course API v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();