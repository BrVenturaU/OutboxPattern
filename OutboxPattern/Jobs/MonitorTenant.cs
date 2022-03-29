using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OutboxPattern.Contexts;
using OutboxPattern.Entities;
using OutboxPattern.Enums;
using OutboxPattern.Interfaces;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OutboxPattern.Jobs
{
    [DisallowConcurrentExecution]
    public class MonitorTenant : IJob
    {
        private readonly ILogger<MonitorTenant> _logger;
        private readonly DataContext _context;
        private readonly IEmailService _emailService;

        public MonitorTenant(ILogger<MonitorTenant> logger, DataContext context, IEmailService emailService)
        {
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Checking Messages");
            var tenantTypeNames = Enum.GetNames<TenantEvent>();
            var messages = await _context.OutboxMessages.Where(o => tenantTypeNames.Contains(o.Type) && !o.IsProcessed)
                .OrderBy(o => o.Data).ToListAsync();
            foreach (var message in messages)
            {
                var data = JsonConvert.DeserializeObject<Tenant>(message.Data);
                await _emailService.SendEmail("user.email@dom.com", "Tracking",
                    $"{message.Type}: Name \"{data.Name}\" at {message.Time.ToShortDateString()}");
                message.MarkAsProcessed(DateTime.Now);
                await _context.SaveChangesAsync();
            }
        }
    }
}
