var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAll(builder.Configuration);
var app = builder.Build();
app.AddMiddleware().Run();


