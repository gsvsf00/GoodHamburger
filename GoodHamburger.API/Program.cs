using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = builder.Configuration["Database:Provider"];

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (provider == "Postgres")
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    else
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

// Register infrastructure services
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<DiscountService>();
builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Categories.Any())
    {
        var sandwich = new Category { Name = "Sanduiche", Code = "SANDWICH" };
        var side = new Category { Name = "Extra", Code = "SIDE" };
        var drink = new Category { Name = "Bebida", Code = "DRINK" };

        context.Categories.AddRange(sandwich, side, drink);
        context.SaveChanges();
    }

    var sandwichCategory = context.Categories.First(c => c.Code == "SANDWICH");
    var sideCategory = context.Categories.First(c => c.Code == "SIDE");
    var drinkCategory = context.Categories.First(c => c.Code == "DRINK");

    if (!context.MenuItems.Any())
    {
        context.MenuItems.AddRange(
            new MenuItem { Name = "X Burger", Price = 5, CategoryId = sandwichCategory.Id },
            new MenuItem { Name = "X Egg", Price = 4.5m, CategoryId = sandwichCategory.Id },
            new MenuItem { Name = "X Bacon", Price = 7, CategoryId = sandwichCategory.Id },
            new MenuItem { Name = "Batata frita", Price = 2, CategoryId = sideCategory.Id },
            new MenuItem { Name = "Refrigerante", Price = 2.5m, CategoryId = drinkCategory.Id }
        );

        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
