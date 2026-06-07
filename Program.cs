var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello Azure world");

app.MapGet("/users", () =>
{
    return new[]
    {
        new { Id = 1, Name = "Elio" },
        new { Id = 2, Name = "John" }
    };
});

app.Run();