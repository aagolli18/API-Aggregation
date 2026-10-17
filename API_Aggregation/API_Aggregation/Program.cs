using API_Aggregation.Interfaces;
using API_Aggregation.Providers;
using API_Aggregation.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
