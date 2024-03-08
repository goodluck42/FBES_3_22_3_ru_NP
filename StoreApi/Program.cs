using Microsoft.EntityFrameworkCore.SqlServer;
using StoreApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/product", (StoreDbContext context) => Results.Ok(context.Products.ToArray()))
    .WithName("GetProducts")
    .WithOpenApi();

app.MapGet("/product/{id:int}", (int id, StoreDbContext context) =>
{
    var result = context.Products.FirstOrDefault(p => p.Id == id);

    return result != null ? Results.Ok(result) : Results.NotFound();
}).WithName("GetProductById")
.WithOpenApi();

app.MapPost("/product", async (Product? product, StoreDbContext context) =>
{
    if (product == null)
    {
        return Results.BadRequest();
    }

    context.Products.Add(product);
    await context.SaveChangesAsync();

    return Results.Ok();
});

app.MapPut("/product/{id:int}", async (int id, Product? product, StoreDbContext context) =>
{
    if (product == null)
    {
        return Results.BadRequest();
    }

    var resultProduct = context.Products.FirstOrDefault(p => p.Id == id);

    if (resultProduct == null)
    {
        return Results.BadRequest();
    }

    resultProduct.Quantity = product.Quantity;

    if (product.Name != null)
    {
        resultProduct.Name = product.Name;
    }

    await context.SaveChangesAsync();
    
    return Results.Ok();
});

app.MapDelete("/product/{id:int}", async (int id, StoreDbContext context) =>
{
    var result = context.Products.FirstOrDefault(p => p.Id == id);

    if (result != null)
    {
        context.Products.Remove(result);

        await context.SaveChangesAsync();
        
        return Results.Ok();
    }

    return Results.BadRequest();
});

app.Run();