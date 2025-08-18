using OpenFgaPoc.Data;
using OpenFgaPoc.Model;
using Service.openFga;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql("Host=db_api;Database=userApi;Username=postgres;Password=admin");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var openFga_ = new OpenFgaConfig();
await openFga_.Init();


var app = builder.Build();

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

app.MapPost("/User/create", (UserModel user, AppDbContext db) =>
{
    db.Users.Add(user);
    db.SaveChanges();
});


app.Run();

