using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Application.Service;
using WaifuAIAssistant.Domain;
using WaifuAIAssistant.Domain.Services;
using WaifuAIAssistant.Infrastructure;
using WaifuAIAssistant.Infrastructure.ThirdParty;
using WaifuAIAssistant.Domain.ThirdPartyInterface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "BlCapstone API Swagger v1", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
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
//builder.Services.AddScoped<ICacheService, RedisCacheService>();


//add redis cache service
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
//    options.InstanceName = "tungdeptrairedis";
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();