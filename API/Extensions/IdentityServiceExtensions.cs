using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            //トークン検証サービス：発行者署名キーに基づいて無効トークンかチェックするサービス
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //署名キー検証有効
                    ValidateIssuerSigningKey = true,
                    //署名キー
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    //APIサーバー側で署名生成する際、発行者情報はトークンにつけていないので、トークンに存在しないので、発行者検証はしない
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            return services;
        }
    }
}