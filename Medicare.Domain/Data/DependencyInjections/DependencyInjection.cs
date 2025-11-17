using Medicare.Domain.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Medicare.Domain.Data.DependencyInjections
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
           this IServiceCollection services,
           string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string not provided");

            services.AddDbContext<MedicareDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                         maxRetryCount: 5,
                         maxRetryDelay: TimeSpan.FromSeconds(30),
                         errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(30);
                });

                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}
