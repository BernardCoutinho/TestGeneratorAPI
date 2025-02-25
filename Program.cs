using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TestGeneratorAPI.src.API.Base.Context;
using TestGeneratorAPI.src.API.Infrastructure.Middlewares;
using TestGeneratorAPI.src.API.Interface.Login;
using TestGeneratorAPI.src.API.Interface;
using TestGeneratorAPI.src.API.Model;
using TestGeneratorAPI.src.API.Repository;
using TestGeneratorAPI.src.API.Service.Auth;
using TestGeneratorAPI.src.API.Service;
using TestGeneratorAPI.src.API.Base;
using TestGeneratorAPI.src.API.Repository.BatchRepository;
using TestGeneratorAPI.src.API.Service.File;
using TestGeneratorAPI.src.API.Repository.FolderRepository;
using TestGeneratorAPI.src.API.Repository.File;
using TestGeneratorAPI.src.API.Helper;
using TestGeneratorAPI.src.API.Config;
using TestGeneratorAPI.src.API.Service.External;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

// Serviços de autenticação, repositories e serviços
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ILoginService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBatchProcessService, BatchProcessService>();
builder.Services.AddScoped<FileProcessingService, FileProcessingService>();
builder.Services.AddScoped<FileContextService, FileContextService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<FolderService, FolderService>();
builder.Services.AddScoped<ExternalApiService, ExternalApiService>();

builder.Services.AddScoped<FolderRepository, FolderRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IBatchProcessRepository, BatchProcessRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<FolderRepository, FolderRepository>();
builder.Services.AddScoped<FileContextRepository, FileContextRepository>();
//builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<FileTransactionHelper, FileTransactionHelper>();

builder.Services.AddControllers();

// Configuração AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();


// HTTP ContextAcessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Configuração do banco de dados SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<PrincipalDbContext>(options =>
options.UseSqlServer(connectionString));
//options.UseInMemoryDatabase("InMemoryDb"));

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do Swagger
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auto Master Api",
        Version = "v1"
    });
    opt.OperationFilter<SwaggerFileOperationFilter>();

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração de autenticação JWT
builder.Services.AddAuthentication((opt) =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer((opt) =>
{
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configuração de logging detalhado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

try
{
    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
    //app.UseHttpsRedirection();

    app.UseMiddleware<UserIdMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseStaticFiles();
    app.Run();
}
catch (HostAbortedException ex)
{
    Console.WriteLine("Host was aborted, but this can be safely ignored.");
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}