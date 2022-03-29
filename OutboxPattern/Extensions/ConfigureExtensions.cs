using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OutboxPattern.Contexts;
using OutboxPattern.Interfaces;
using OutboxPattern.Jobs;
using OutboxPattern.Services;
using OutboxPattern.Settings;
using Quartz;

namespace OutboxPattern.Extensions
{
    public static class ConfigureExtensions
    {
        public static IServiceCollection ConfigureAppServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        public static IServiceCollection ConfigureDb(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            return services;
        }

        public static IServiceCollection ConfigureJobs(this IServiceCollection services)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                var key = new JobKey($"{nameof(MonitorTenant)}_job");
                options.AddJob<MonitorTenant>(jobOptions => jobOptions.WithIdentity(key));

                options.AddTrigger(triggerOptions => triggerOptions
                    .ForJob(key)
                    .WithIdentity($"{nameof(MonitorTenant)}_trigger")
                    .WithCronSchedule("0/10 * * * * ?")
                );
            });
            services.AddQuartzHostedService(h => h.WaitForJobsToComplete = true);
            return services;
        }

        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            services.AddScoped<IEmailSettings>(sp => sp.GetRequiredService<IOptions<EmailSettings>>().Value);
            return services;
        }
    }
}
