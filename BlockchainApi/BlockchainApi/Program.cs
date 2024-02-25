
using Backend.Behaviours;
using Backend.Shared.Extensions;
using BlockchainApi.Shared.Extensions.Services;
using MediatR.Pipeline;
using System.Reflection;

namespace BlockchainApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var assembly = typeof(Program).Assembly;
        builder.AddIdentity();
        builder.AddDatabase();
        builder.Services.AddHealthChecks();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddValidatorsFromAssembly(assembly);

        builder.Services.AddCarter();

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(TransactionScopeBehaviour<,>));
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //builder.Services.AddHostedService<AttackWatcherHostedService>();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();

        await app.InitialiseDatabaseAsync();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapCarter();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        //app.MapHealthChecks("/health");

        app.MapControllers();

        app.Run();
    }
}
