using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyAzureDemo.Data;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString =
            FirstConfiguredValue(
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"),
                Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection"))
            ?? "Host=localhost;Database=myazuredemo;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string? FirstConfiguredValue(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }
}
