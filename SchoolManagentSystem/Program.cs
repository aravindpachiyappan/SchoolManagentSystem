using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using SchoolManagentSystem_API.DBContext;
using System.Text;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        // ─── Database ────────────────────────────────────────────────────────────────
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
                   .EnableDetailedErrors(builder.Environment.IsDevelopment()));

        // ─── JWT Authentication ───────────────────────────────────────────────────────
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAud = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAud,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero   // No grace period on token expiry
            };

            // Return 401 JSON instead of redirecting
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(
                        """{"error":"Unauthorized","message":"A valid JWT token is required."}""");
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(
                        """{"error":"Forbidden","message":"You do not have permission to access this resource."}""");
                }
            };
        });

        // ─── Authorization ───────────────────────────────────────────────────────────
        builder.Services.AddAuthorization();

        // ─── CORS ────────────────────────────────────────────────────────────────────
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {

                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();

            });
        });

        // ─── Swagger ─────────────────────────────────────────────────────────────────
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Student Management API",
                Version = "v1",
                Description = "REST API for managing students, courses, and enrollments."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: Bearer eyJhbGci..."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        // ─── Build ───────────────────────────────────────────────────────────────────
        var app = builder.Build();

        // ─── Auto-migrate on startup (dev only) ──────────────────────────────────────
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
        }

        // ─── Pipeline ────────────────────────────────────────────────────────────────
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
                c.RoutePrefix = string.Empty; // Swagger at root: https://localhost:PORT/
            });
        }
        else
        {
            // Global error handler in production
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");       // ← Must be before Auth
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Minimal error endpoint for production
        app.Map("/error", () =>
            Results.Problem(title: "An unexpected error occurred.", statusCode: 500));

        app.Run();
    }
}