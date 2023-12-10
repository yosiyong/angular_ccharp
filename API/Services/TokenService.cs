using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        //サーバー側に保持される署名の復号用対称キー（非対称キーは暗号化する際、使うキー）
        //クライアントに送信されないこと
        private readonly SymmetricSecurityKey _key;
        //設定情報を取得
        public TokenService(IConfiguration config)
        {
            //設定ファイルの値からキー生成
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //署名内容
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            //署名暗号化
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //*返すトークン生成
            //トークンに署名
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            //トークンハンドラー
            var tokenHandler = new JwtSecurityTokenHandler();

            //トークン生成
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}