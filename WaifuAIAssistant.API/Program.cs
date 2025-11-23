using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;
using WaifuAIAssistant.API.Middleware;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Application.Service;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Services;
using WaifuAIAssistant.Domain.ThirdPartyInterface;
using WaifuAIAssistant.Infrastructure;
using WaifuAIAssistant.Infrastructure.ThirdParty;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();


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
    });
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
//builder.Services.AddScoped<ICacheService, RedisCacheService>();


//add redis cache service
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
//    options.InstanceName = "tungdeptrairedis";
//});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DefaultConnection"),

        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 15,                    // chuẩn doanh nghiệp
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // 3 option BẮT BUỘC phải có với Azure SQL (rất nhiều người bỏ quên)
            //sqlOptions.CommandTimeout(60);
            //sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            //sqlOptions.ExecutionStrategy(d => new SqlServerRetryingExecutionStrategy(d, 15, TimeSpan.FromSeconds(30), null));
        })
    .EnableSensitiveDataLogging(false)
    .EnableDetailedErrors(false));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();