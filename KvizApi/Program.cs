using KvizApi.Models;
using KvizApi.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// Disable HTTPS in non-development environments
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5000); // HTTP
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps(); // Only enable HTTPS locally if dev certificate is set
        });
    });
}
else
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(80); // Only HTTP in production
    });
}
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // OpenAPI docs

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5032); // HTTP
    options.ListenAnyIP(7169, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});

builder.Services.AddSingleton<QuestionService>();

var app = builder.Build();

// If the app is running in development, you can enable HTTPS
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll"); //

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
