using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using QualiTrack.Data;
using QualiTrack.Middlewares;
using Microsoft.IdentityModel.Logging;

IdentityModelEventSource.ShowPII = true;

var builder = WebApplication.CreateBuilder(args);

// ===============================================================================
// 1. DATABASE
// ===============================================================================
var connectionString = builder.Configuration.GetConnectionString("Supabase")
    ?? throw new InvalidOperationException("Connection string 'Supabase' not found.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// ===============================================================================
// 2. JWT AUTH (🔥 CLEAN VERSION)
// ===============================================================================
var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSection["Key"]!;
var jwtIssuer = jwtSection["Issuer"]!;
var jwtAudience = jwtSection["Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };

        opt.Events = new JwtBearerEvents{
            OnMessageReceived = context =>{
                var authHeader = context.Request.Headers["Authorization"].ToString();

                Console.WriteLine("==== HEADER AUTH ====");
                Console.WriteLine(authHeader);

                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ")){
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }

                Console.WriteLine("Token Parsed:");
                Console.WriteLine(context.Token);
                Console.WriteLine("=====================");

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>{
                Console.WriteLine("===== JWT FAILED =====");
                Console.WriteLine("Error: " + context.Exception.Message);

                // ambil header langsung (BUKAN context.Token)
                var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
                Console.WriteLine("Header saat gagal:");
                Console.WriteLine(authHeader);

                Console.WriteLine("=====================");

                return Task.CompletedTask;
            }
        };
    });

// ===============================================================================
// 3. AUTHORIZATION
// ===============================================================================
builder.Services.AddAuthorization();

// ===============================================================================
// 4. CORS
// ===============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ===============================================================================
// 5. CONTROLLERS
// ===============================================================================
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ===============================================================================
// 6. OPENAPI (BUILT-IN .NET 10)
// ===============================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ===============================================================================
// 7. BUILD
// ===============================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("DefaultCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();