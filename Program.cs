var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello akhi");

app.MapGet("/users", () =>
{
    return new[]
    {
        new { Id = 1, Name = "mtm" },
        new { Id = 2, Name = "John" }
    };
});

app.Run();