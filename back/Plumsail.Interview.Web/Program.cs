using System.Text.Json.Serialization;

using Plumsail.Interview.Handlers.SubmissionHandlers;
using Plumsail.Interview.MemoryDatabase;
using Plumsail.Interview.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureFormLimits(builder.Configuration);

builder.Services.AddPlumsailDependencies();
builder.Services.AddInMemoryDatabase();

builder.Services.AddCorsPolicy();
builder.Services.AddSwagger();

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(SubmissionsGetRequest).Assembly];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

var app = builder.Build();

await app.EnsureDatabaseCreatedAsync();

app.UseSwaggerWithUI();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();