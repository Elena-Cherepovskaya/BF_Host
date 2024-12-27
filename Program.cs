using BF_Host.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<UserService>(
   builder.Configuration.GetSection("UserService"));

builder.Services.AddSingleton<UserService>();

builder.Services.AddSingleton<RecipeService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<IngredientService>();

builder.Services.AddSingleton<StorageProductService>();
builder.Services.AddSingleton<StorageIngredientService>();

builder.Services.AddSingleton<IngredientForRecipeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
