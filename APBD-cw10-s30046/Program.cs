using APBD_cw10_s30046.Data;
using APBD_cw10_s30046.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ClientAndTripsDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();



app.UseAuthorization();

app.MapControllers();

app.Run();