using System.Text.Json.Serialization; 
using CompanyERP.IServices;
using CompanyERP.Profiles;
using CompanyERP.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// ?? 1. ???? ????? ??????? ??? ??? AddOpenApi
builder.Services.AddEndpointsApiExplorer();

// ????? ???? ????? ????? ???? ?? ??????????
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<CompanyERP.Entities.Employee>, Microsoft.AspNetCore.Identity.PasswordHasher<CompanyERP.Entities.Employee>>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CompanyERP API", Version = "v1" });

    // ????? ??? ??? JWT Bearer ?? ????? ??? Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");
builder.Services.AddDbContext<CompanyERP.Data.AppDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeProjectService, EmployeeProjectService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<CompanyERP.Security.TokenGenerator>();

// 1. ????? ??????? ??? JWT ?????? ?? ??????? ?????? ????????? ??????
var jwtSettingsSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<CompanyERP.DTOs.JwtSettings>(jwtSettingsSection);

// 2. ????? ???? ??? Authentication ?????? ??? ??????? ????? ??? Jwt Bearer Tokens
var jwtSettings = jwtSettingsSection.Get<CompanyERP.DTOs.JwtSettings>();
var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings!.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero 
    };
});


var app = builder.Build();

app.UseMiddleware<CompanyERP.Exceptions.ExceptionMiddleware>();

// ?? 2. ????? ??????? ??????? ??? ??? If ???? ????? ?????
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

app.Run();