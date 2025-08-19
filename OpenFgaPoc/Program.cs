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


app.MapGet("/User", (AppDbContext db) =>
{
    return db.Users.ToList();
});

await openFga_.CreateTuple("papel:pesquisador","endpoint:pesquisador_post","papel");
app.MapPost("/acesso/pesquisador", (AppDbContext db, string name) =>
{
    string user_cargo = db.Users.Where(x => x.Name == name).FirstOrDefault().Cargo;
    if (openFga_.CheckTuple(name, "endpoint:pesquisador_post", "accessible").Result)
    {
        return "Acesso Permitido";
    }
    return "Acesso Negado";
});

await openFga_.CreateTuple("papel:bolsista","endpoint:bolsista_post","papel");
app.MapPost("/acesso/bolsista", (AppDbContext db, string name) =>
{
    string user_cargo = db.Users.Where(x => x.Name == name).FirstOrDefault().Cargo;
    if (openFga_.CheckTuple(name, "endpoint:bolsista_post", "accessible").Result)
    {
        return "Acesso Permitido";
    }
    return "Acesso Negado";
});

await openFga_.CreateTuple("papel:bolsista", "endpoint:acesso_post", "papel");
await openFga_.CreateTuple("papel:pesquisador", "endpoint:acesso_post", "papel");
app.MapPost("/acesso", async (AppDbContext db, string name) =>
{
    string user_cargo = db.Users.Where(x => x.Name == name).FirstOrDefault().Cargo;
    if(openFga_.CheckTuple(name, "endpoint:acesso_post", "accessible").Result)
    {
        return "Acesso Permitido";
    }
    return "Acesso Negado";
});

app.MapPost("/User/create", async (UserModel user, AppDbContext db) =>
{   
    db.Users.Add(user);
    await openFga_.CreateTuple($"user:{user.Name.ToLower()}", $"papel:{user.Cargo.ToLower()}", "rw");
    await db.SaveChangesAsync();

    return Results.Ok();
});


app.Run();

