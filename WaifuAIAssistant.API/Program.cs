using Mapster;
using MapsterMapper;
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
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DefaultConnection"),

        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 15,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // 3 option BẮT BUỘC phải có với Azure SQL (rất nhiều người bỏ quên)
            //sqlOptions.CommandTimeout(60);
            //sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            //sqlOptions.ExecutionStrategy(d => new SqlServerRetryingExecutionStrategy(d, 15, TimeSpan.FromSeconds(30), null));
        })
    .EnableSensitiveDataLogging(false)
    .EnableDetailedErrors(false));  

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();