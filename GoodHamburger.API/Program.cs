using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

    if (!context.MenuItems.Any())
    {
        context.MenuItems.AddRange(
            new MenuItem { Name = "X Burger", Price = 5, Type = ProductType.Sandwich },
            new MenuItem { Name = "X Egg", Price = 4.5m, Type = ProductType.Sandwich },
            new MenuItem { Name = "X Bacon", Price = 7, Type = ProductType.Sandwich },
            new MenuItem { Name = "Fries", Price = 2, Type = ProductType.Side },
            new MenuItem { Name = "Soda", Price = 2.5m, Type = ProductType.Drink }
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
