using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using 小專後端.Dtos;
using 小專後端.Dtos.User;
using 小專後端.Models;
namespace 小專後端.Services
{
    public class UsersDBServices
    {
        private readonly IConfiguration _englishContext;//這是讀取設定檔的
        private readonly englishContext _dbContext;//這是讀取資料庫用的
        private readonly MailService _mailService;//存gmail
        public UsersDBServices(IConfiguration configuration, englishContext dbContext, MailService mailService)//IConfiguration用來讀取appstring裡面的設定
        {
            _englishContext = configuration;
            _dbContext = dbContext;
            _mailService = mailService;
        }
        public string GenerateToken(int uid, string email, string role)
        {
            var issuer = _englishContext.GetValue<string>("JwtSettings:Issuer");
            var signKey = _englishContext.GetValue<string>("JwtSettings:SignKey");
            var ExpireMinutes = _englishContext.GetValue<double>("JwtSettings:ExpireMinutes");
            //從appstering設定檔裡面抓_englishContext資料

            var claims = new List<Claim>//填寫個人資料
            {
               new Claim(JwtRegisteredClaimNames.Sub, uid.ToString()),
               new Claim(JwtRegisteredClaimNames.Email, email),
               new Claim(ClaimTypes.Role, role ?? "User"),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//防偉造印章(加密簽名)


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(ExpireMinutes),
                SigningCredentials = signingCredentials
            };

            //把資料打包成token
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
        public string LoginCheck(string email, string password)//LOGIN
        {
            users data = GetDataByEmail(email);

            if (data == null || HashPwd(password) != data.password)
            {
                return null;
            }

            string token = GenerateToken(data.uid, data.email, data.role);//如果登入成功就產生一個JWT token，裡面包含使用者的ID、Email和角色等資訊，然後回傳這個token給前端，前端就可以在之後的API請求中帶著這個token來驗證使用者的身份和權限
            return token;
        }
        public users GetDataByEmail(string email)
        {
            users data = _dbContext.users.FromSqlInterpolated($"SELECT * FROM users WHERE email = {email}").FirstOrDefault();
            return data;
        }
        private string HashPwd(string pwd)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                string saltkey = "edsr6h5ge6r68468rgd6ser54g65";
                string saltAndPwd = String.Concat(pwd, saltkey);
                var bytes = Encoding.UTF8.GetBytes(saltAndPwd);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public string Register(UserRegisterDto RegisterData)
        {
            string authCode = Guid.NewGuid().ToString().Substring(0, 8); //產生驗證碼(會藏在連結驗證裡面)
            _dbContext.Database.ExecuteSqlInterpolated($@" INSERT INTO users (email, password, nickname, role, created_at, auth_code) 
                 VALUES ({RegisterData.email}, {HashPwd(RegisterData.password)}, {RegisterData.nickname}, 'user', GETDATE(), {authCode});");
            return authCode;
        }
        public bool EmailCheck(string email)
        {
            //藉由傳入帳號取得會員資料
            users Data = GetDataByEmail(email);
            //判斷是否有查詢到會員
            bool result = (Data == null);
            //回傳結果
            return result;
        }
        public void ChangePwd(string email, string newPassword)
        {
            _dbContext.Database.ExecuteSqlInterpolated($@"UPDATE users SET password = {HashPwd(newPassword)} WHERE email LIKE {email};");
        }

        public string EmailValidate(string authCode, int uid)
        {
            var user = _dbContext.users.FirstOrDefault(u => u.auth_code == authCode);
            _dbContext.Database.ExecuteSqlInterpolated($"UPDATE users SET auth_code = {authCode} WHERE uid = {uid}");

            if (user != null)
            {
                user.auth_code = null;
                _dbContext.SaveChanges();
                return "驗證成功";
            }
            else
            {
                return "驗證失敗，連結可能已過期或驗證碼不正確";
            }
        }
        public string GenerateResetToken(string email)//產生忘記密碼的token
        {
            var user = _dbContext.users.FirstOrDefault(u => u.email == email);
            if (user == null) return null;

            string token = Guid.NewGuid().ToString().Substring(0, 8);
            _dbContext.Database.ExecuteSqlInterpolated($"UPDATE users SET auth_code = {token} WHERE uid = {user.uid}");

            return token;
        }
        public bool ValidateResetToken(int uid, string authCode)//驗證忘記密碼的token是否有效
        {
            var user = _dbContext.users.FirstOrDefault(u => u.uid == uid && u.auth_code == authCode);

            return user != null;
        }
        public bool ResetPasswordWithToken(Userforgetpassword resetData)//這個方法是用來處理使用者提交的忘記密碼重設資料
        {
            if (string.IsNullOrEmpty(resetData.auth_code)) return false;

            var user = _dbContext.users
                .FromSqlInterpolated($"SELECT * FROM users WHERE uid = {resetData.uid} AND auth_code = {resetData.auth_code}")
                .FirstOrDefault();

            if (user != null)
            {
                string newPwd = resetData.New_password;

                _dbContext.Database.ExecuteSqlInterpolated($"UPDATE users SET password = {HashPwd(newPwd)}, auth_code = '' WHERE uid = {resetData.uid}");
                return true;
            }

            return false;
        }
        //個人資料
        public UserProfileDto GetUserProfile(int uid)
        {
            var user = _dbContext.users.FirstOrDefault(u => u.uid == uid);
            if (user == null) return null;

            return new UserProfileDto
            {
                uid=user.uid,
                email = user.email,
                nickname = user.nickname,
                created_at = user.created_at,
                role = user.role
            };
        }

        public List<UserProfileDto> GetUserProfiles()
        {
            List<users> users= _dbContext.users.ToList(); ;
            if (users == null) return null;
            List<UserProfileDto> userDtos = new List<UserProfileDto>();

            foreach (var item in users) {
                UserProfileDto userDto = new UserProfileDto();
                userDto.uid = item.uid;
                userDto.email = item.email;
                userDto.nickname = item.nickname;
                userDto.created_at = item.created_at;
                userDto.role = item.role;
                userDtos.Add(userDto);
            }

            return userDtos;
        }
        public string UpdateUserProfile(int uid, UpdateProfileDto dto)
        {
            var user = _dbContext.users.FirstOrDefault(u => u.uid == uid);
            if (user == null) return "找不到該使用者";

            if (string.IsNullOrWhiteSpace(dto.nickname))
            {
                return "暱稱不能為空";
            }

            bool isEmailChanged = user.email != dto.email;
            bool isNicknameChanged = user.nickname != dto.nickname;

            if (!isEmailChanged && !isNicknameChanged)
            {
                return "資料無變動";
            }

            if (isEmailChanged)
            {
                var emailExists = _dbContext.users.Any(u => u.email == dto.email && u.uid != uid);
                if (emailExists) return "此電子郵件已被其他帳號使用";

                string newAuthCode = Guid.NewGuid().ToString().Substring(0, 8);
                user.auth_code = newAuthCode;
                _mailService.SendEmailChangeVerifyMail(dto.nickname, dto.email, newAuthCode, uid);
            }

            user.email = dto.email;
            user.nickname = dto.nickname;

            _dbContext.SaveChanges();
            if (isEmailChanged)
            {
                return "信箱已更改，請至新信箱收取驗證信並重新登入";
            }
            else
            {
                return "暱稱更新成功";
            }
        }

        public string UpdateRole(UpdateRoleDto dto)//這個方法是用來更新使用者的角色，只有管理員可以使用這個方法來更改其他使用者的角色
        {
            try
            {
                var user = _dbContext.users.FirstOrDefault(u => u.uid == dto.uid);

                if (user != null)
                {
                    user.nickname = dto.nickName;
                    user.role = dto.role;
                    _dbContext.SaveChanges();
                }
                return "更改成功";
            }
            catch (Exception ex) { 
                return ex.Message;
            }
        }
        public async Task<int> GetTotalUsersCountAsync()
        {
            try
            {
                var totalCount = await _dbContext.users.CountAsync();

                return totalCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}