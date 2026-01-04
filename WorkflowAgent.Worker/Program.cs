using Microsoft.EntityFrameworkCore;
using WorkflowAgent.Infrastructure.Persistence;
using WorkflowAgent.Worker;
using WorkflowAgent.Core.AI;
using WorkflowAgent.Infrastructure.AI;


var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WorkflowAgentDb");
builder.Services.AddDbContext<WorkflowAgentDbContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddSingleton<ITriageService, StubTriageService>();
builder.Services.AddHostedService<TicketJobProcessor>();



var host = builder.Build();
host.Run();
