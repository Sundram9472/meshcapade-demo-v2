using MeshcapadeDemo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(e=>new HttpClient { BaseAddress=new Uri("https://api.meshcapade.com/api/v1/") });
builder.Services.AddScoped(typeof(MeshcapadeService));
builder.Services.AddCors();

var app = builder.Build();



    app.UseSwagger();
    app.UseSwaggerUI();

app.UseCors(e=>e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
