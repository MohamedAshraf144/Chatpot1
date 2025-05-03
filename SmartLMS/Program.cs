using Microsoft.EntityFrameworkCore;
using SmartLMS.Application.Services;
using SmartLMS.Core.Interfaces;
using SmartLMS.Infrastructure.Data;
using SmartLMS.Infrastructure.Repositories;
using SmartLMS.API.Middleware;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ????? ????? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// ????? ?????????? ?????? ????????? - ????? ??????? ???????
// ?????? ?????? ?? ???????? ?????? ???? ??????
// ????????:
//builder.Services.AddScoped<IChatbotService, MockChatbotService>();
// ???????:
//builder.Services.AddHttpClient<IChatbotService, ClaudeChatbotService>();
//builder.Services.AddHttpClient<IChatbotService, GoogleGeminiChatbotService>();
// ????? ??????? Hugging Face ?? appsettings.json
builder.Services.AddHttpClient<IChatbotService, HuggingFaceChatbotService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatService, ChatService>();

// ????? AutoMapper
builder.Services.AddAutoMapper(typeof(SmartLMS.Application.Mappings.MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseDiagnostics();
app.MapControllers();

app.Run();