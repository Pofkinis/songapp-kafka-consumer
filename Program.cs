using MessageProcessor.Models;
using MessageProcessor.Repositories;
using MessageProcessor.Repositories.Interfaces;
using MessageProcessor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;


// Set up dependency injection
    var serviceProvider = ConfigureServices();
    
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

    // Resolve the service
    using (var serviceScope = serviceProvider.CreateScope())
    {
        var service = serviceScope.ServiceProvider.GetRequiredService<IKafkaService>();

        // Execute the logic in the service
        await service.ProcessMessages();
    }


static IServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();

    services.AddDbContext<DatabaseContext>(options =>
        options.UseMySQL("server=127.0.0.1;port=3306;user=root;password=;database=songs;"));
    
    services.AddScoped<IKafkaService, KafkaService>();
    services.AddTransient<IUserSongRepository, UserSongRepository>();

    return services.BuildServiceProvider();
}