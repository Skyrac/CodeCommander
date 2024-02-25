using Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Migrations;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task MigrateAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public void DeleteRows()
    {
        var tableNames = _context.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Distinct()
            .ToList();
        var i = 0;
        while (tableNames.Count > 0)
        {
            if (i >= tableNames.Count)
            {
                i = 0;
            }
            var tableName = $"\"public\".\"{tableNames[i]}\"";
            try
            {
                _context.Database.ExecuteSqlRaw($"DELETE FROM {tableName};");
                tableNames.RemoveAt(i);
            }
            catch (Exception ex)
            {
                i++;
            }
        }
    }


    public TEntity AddEntityToDatabase<TEntity>(TEntity entity)
    where TEntity : class
    {

        var entry = _context.Add(entity);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        return entry.Entity;
    }

    public void AddEntitiesToDatabase<TEntity>(IEnumerable<TEntity> entity)
        where TEntity : class
    {

        _context.AddRange(entity);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

}
