using Mapster;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using WaifuAIAssistant.API;
using WaifuAIAssistant.API.Middleware;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Application.Service;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Services;
using WaifuAIAssistant.Domain.ThirdPartyInterface;
using WaifuAIAssistant.Infrastructure;
using WaifuAIAssistant.Infrastructure.ThirdParty;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(opts =>
    opts.Conventions.Add(new RouteTokenTransformerConvention(new ToKebabParameterTransformer()))) // make URL become kebab case
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:3000", "https://localhost:3000" };

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add configuration from appsettings.json, environment variables, and user secrets
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Waifu AI API Swagger v1", Version = "v1", Description = "A full API for project" });
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    }); // add bearer token support
    options.EnableAnnotations(); // enable swagger annotations
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["waifu_access_token"];
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DefaultConnection");

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);

        sqlOptions.CommandTimeout(60);
    });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

//add redis cache service
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.ConnectAsync(connectionString).GetAwaiter().GetResult();
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("messagePolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.AddPolicy("loginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});


builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IPasswordHandlerService, PasswordHandlerService>();
builder.Services.AddScoped<IJwtService, JWTService>();
builder.Services.AddScoped<IAuthCookieService, AuthCookieService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<GoogleService>();
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IModelsCharacterService, ModelsCharacterService>();
builder.Services.AddScoped<ICharacterEmotionService, CharacterEmotionService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IGenerationAIService, GenerationAIService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

//config Mapster object
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(MapObject).Assembly);

builder.Services.AddSingleton(config);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.UseHealthChecks("/health", new HealthCheckOptions
{
    
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                exception = entry.Value.Exception?.Message,
                duration = entry.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.Run();