using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt => opt.UseSqlite(config.GetConnectionString("DefaultConnection")));
            services.AddCors();

            //有効期間がクライアントリクエストごとに 1 回作成され、スコープ付きサービスの最後の処理（レスポンス時）に破棄される。
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}