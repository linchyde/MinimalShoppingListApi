using Microsoft.EntityFrameworkCore;
using MinimalShoppingListApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(options => options.UseInMemoryDatabase("ShoppingListApi"));

var app = builder.Build();

//below line will return all the in memory groceries
//this is an endpoint
app.MapGet("/shoppinglist", async (ApiDbContext db) => await db.Groceries.ToListAsync());

app.MapPut("/shoppinglist/{id}", async (int id, Grocery grocery, ApiDbContext db) =>
{
    var groceryIndb = await db.Groceries.FindAsync(id);

    if(groceryIndb!=null)
    {
        groceryIndb.Name = grocery.Name;
        groceryIndb.Purchased= grocery.Purchased;
        await db.SaveChangesAsync();
        return Results.Text("Grocery Updated");
    }

    return Results.NotFound();
});

app.MapDelete("/shoppinglist/{id}", async (int id, ApiDbContext db) =>
{
    var groceryFromDb = await db.Groceries.FindAsync(id);

    if(groceryFromDb!= null)
    {
        db.Groceries.Remove(groceryFromDb);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.MapGet("/shoppinglist/{id}", async (int id, ApiDbContext db) =>
{
    var groceryFromDb = await db.Groceries.FindAsync(id);

    return groceryFromDb != null ? Results.Ok(groceryFromDb): Results.NotFound();

});

app.MapPost("/shoppinglist", async (Grocery grocery, ApiDbContext db) =>
{
    db.Groceries.Add(grocery);

    await db.SaveChangesAsync();

    return Results.Created($"/shoppinglist/{grocery.Id}", grocery);
});

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();