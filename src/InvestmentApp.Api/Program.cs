using InvestmentApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Allow the frontend dev server to call this API (e.g. http://localhost:3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper profiles (optional: keeps DI-based mapping available)
builder.Services.AddAutoMapper(cfg => { }, typeof(InvestmentApp.Api.Mappers.MappingProfile).Assembly);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    // Show developer exception page in Development so test server prints exception details
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

    app.UseHttpsRedirection();

    app.UseCors("LocalDev");

    app.UseAuthorization();

    app.MapControllers();

app.Run();