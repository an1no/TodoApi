using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. - AddService
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline. - UseMethod....
app.MapGet("/todoitems", async (TodoDb db) =>
{ 
    return await db.Todos.ToListAsync();
});

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
{
    return await db.Todos.FindAsync(id);
});

app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem todo, TodoDb db) =>
{
    var existingTodo = await db.Todos.FindAsync(id);
    if (existingTodo is null) return Results.NotFound();

    existingTodo.Name = todo.Name;
    existingTodo.IsComplete = todo.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();