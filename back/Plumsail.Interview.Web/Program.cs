using System.Text.Json.Serialization.Metadata;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Handlers;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using Plumsail.Interview.LiteMemoryDatabase;
using Plumsail.Interview.Web.Bindings;
using Plumsail.Interview.Web.Controllers;
using Plumsail.Interview.Web.Extensions;

AotKeepTypes.EnsureTypesArePreserved();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opts =>
{
    opts.SerializerOptions.TypeInfoResolver = JsonTypeInfoResolver.Combine(
        AppJsonSerializerContext.Default,
        HandlersJsonSerializerContext.Default);
});

builder.Services.AddPlumsailDependencies();
builder.Services.AddLiteMemoryDatabase();

builder.Services.AddCorsPolicy();
builder.Services.AddScalar();

builder.Services.AddMediator(options =>
{
    options.Assemblies = [typeof(SubmissionsGetRequest).Assembly];
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

var app = builder.Build();

await app.EnsureDatabaseCreatedAsync();

app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseCorsPolicy();

app.MapSubmissionEndpoints();

app.UseScalar();

await app.RunAsync();