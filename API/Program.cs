var app = WebApplication
        .CreateBuilder(args)
        .AddServices()
        .Build();

app.AddMiddleware();
await app.SeedDatabaseAsync();
app.Run();


