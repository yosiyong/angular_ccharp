using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

//Configure the HTTP request pipeline.
//API側からレスポンスにAccess Control Allow Origin ヘッダーをつける
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseHttpsRedirection();

//有効トークン
app.UseAuthentication();
//トークン認証
app.UseAuthorization();

app.MapControllers();

app.Run();
