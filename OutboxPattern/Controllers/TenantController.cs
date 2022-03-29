using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OutboxPattern.Contexts;
using OutboxPattern.Entities;
using OutboxPattern.Enums;
using OutboxPattern.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutboxPattern.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly DataContext _context;

        public TenantController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tenant>>> Get()
        {
            var tenants = await _context.Tenants.ToListAsync();

            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetById(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if(tenant == null)
                return NotFound("Data was not found.");
            
            return Ok(tenant);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Tenant tenant)
        {            
            var json = JsonConvert.SerializeObject(new { Name = tenant.Name});
            var outboxMessage = new OutboxMessage(Guid.NewGuid(), DateTime.Now, TenantEvent.Created.ToString(), json);
            _context.Add(tenant);
            _context.Add(outboxMessage);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = tenant.Id}, tenant);
        }

    }
}
