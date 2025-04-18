using FastEndpoints;
using GymApp.Core.Common;
using GymApp.Core.Interfaces;
using GymApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddFastEndpoints();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
builder.Services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseFastEndpoints();
app.UseHttpsRedirection();

app.Run();
