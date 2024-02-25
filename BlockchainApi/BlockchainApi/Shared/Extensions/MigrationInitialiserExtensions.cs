using Backend.Migrations;

namespace Backend.Shared.Extensions;

public static class MigrationInitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        if (app.Environment.IsDevelopment())
        {

            await initialiser.MigrateAsync();

            initialiser.DeleteRows();
        }
        else
        {
            await initialiser.MigrateAsync();
        }
    }
}
