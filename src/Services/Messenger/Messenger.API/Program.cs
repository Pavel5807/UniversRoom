using UniversRoom.Services.Common;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
// builder.Services.AddHealthChecks(builder.Configuration);
// builder.Services.AddDbContexts(builder.Configuration);
// builder.Services.AddApplicationOptions(builder.Configuration);
// builder.Services.AddIntegrationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();


app.Run();
