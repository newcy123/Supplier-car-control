using SupplierControlBackend.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddDbContext<DbSC_Suppiler_Driver_Lisense>();
builder.Services.AddDbContext<DbSC_SC_Suplier_In_Out>();
builder.Services.AddDbContext<DbAL_vender>();
builder.Services.AddCors(options => options.AddPolicy("Cors", builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));
var app = builder.Build();
app.UseCors("Cors");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
