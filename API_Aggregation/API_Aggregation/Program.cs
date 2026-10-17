using API_Aggregation.BackgroundServices;
using API_Aggregation.Interfaces;
using API_Aggregation.Providers;
using API_Aggregation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<DummyJsonApiProvider>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com/");
});

builder.Services.AddHttpClient<PokeApiProvider>(client =>
{
    client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
});

builder.Services.AddHttpClient<JsonPlaceholderApiProvider>(client =>
{
    client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
});

builder.Services.AddTransient<IApiProvider>(sp => sp.GetRequiredService<DummyJsonApiProvider>());
builder.Services.AddTransient<IApiProvider>(sp => sp.GetRequiredService<PokeApiProvider>());
builder.Services.AddTransient<IApiProvider>(sp => sp.GetRequiredService<JsonPlaceholderApiProvider>());
builder.Services.AddScoped<AggregationService>();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IApiStatisticsService, ApiStatisticsService>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

    options.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                "Bearer", document)] = []
        });
});

builder.Services.AddScoped<IAuthService, AuthService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthorization();
builder.Services.AddHostedService<ApiMonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
