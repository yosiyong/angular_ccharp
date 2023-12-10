using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
              this._context = context;
            this._tokenService = tokenService;
        }

        [HttpPost("register")]  //Post: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                //HMACは一方向ハッシュ関数を用いてメッセージ認証コードを構成する手法
                //パスワード文字列からSHA512で鍵のハッシュ値を求め（ランダム生成）、そのハッシュ値をあらためてHMACの鍵とする
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //*ユーザー存在チェック
            //single:取得データが複数の場合は例外(firstは複数データ中1番目のデータを取得)
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            //指定ユーザーがない場合、非承認状態にする。(Unauthorizedを使うにはAPI戻り値をActionResultにする必要がある)
            if (user == null) return Unauthorized("invalid username");

            //*パスワード検証
            //キーからHash取得
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //パスワードチェック
            for (int i = 0; i < ComputeHash.Length; i++)
            {
                if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}