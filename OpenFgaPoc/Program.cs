using OpenFgaPoc.Data;
using OpenFgaPoc.Model;
using Service.openFga;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://+:5002", "https://+:5001");


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var openFga_ = new OpenFgaConfig();
await openFga_.Init();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/AuthModelId", () =>
{
    return openFga_.AuthModelId;
});

app.MapPost("/tupleUser", (string name, string obj, string relation) =>
{
    return openFga_.CreateTuple(name,obj,relation);
});

app.MapPost("/CheckUser", (string name, string obj, string relation) =>
{
    return openFga_.CheckTuple(name,obj,relation);
});

app.MapPost("/User/create", async (UserModel user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.Run();

