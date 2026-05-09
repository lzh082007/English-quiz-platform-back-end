using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using 小專後端.Dtos;
using 小專後端.Dtos.User;
using 小專後端.Models;
using 小專後端.Services;

namespace 小專後端.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly UsersDBServices _UserDbContext;
        private readonly MailService _mailService;
        public UserController(UsersDBServices UserDbContext, MailService mailService) {
            _UserDbContext = UserDbContext;
            _mailService = mailService;
        }

        [HttpPost("login")]//登入
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var token = _UserDbContext.LoginCheck(dto.email,dto.password);

            if (token == null)
            {
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            users data = _UserDbContext.GetDataByEmail(dto.email);

            if (!string.IsNullOrEmpty(data.auth_code))
            {
                return Unauthorized(new {message = "帳號尚未驗證" });
            }

            return Ok(new
            {
                message = "已登入",
                token = token,
            });
        }
        [HttpPost("Register")]//註冊
        public IActionResult Register([FromBody] UserRegisterDto RegisterData)
        {
            if (!ModelState.IsValid)
            {
               var errorMessages = ModelState.Values .SelectMany(v => v.Errors).Select(e => e.ErrorMessage) .ToList();
                return BadRequest(new
                {
                  message = "註冊資料格式錯誤",
                  errors = errorMessages
                });
            }
            try
            {
                 if (!_UserDbContext.EmailCheck(RegisterData.email))
                 {
                    return BadRequest(new { message = "此Email被註冊過了" });
                 }

                string authCode = _UserDbContext.Register(RegisterData);//註冊成功後會回傳驗證碼
                var newUser = _UserDbContext.GetDataByEmail(RegisterData.email);//根據email拿到剛註冊的使用者資料，從中取出uid
                int uid = newUser.uid;

                _mailService.SendRegisterMail(RegisterData.nickname, RegisterData.email, authCode,uid);

                 return Ok(new { message = "註冊成功，請去郵件收信" });
            }
            catch (Exception ex)
            {
               return StatusCode(500, new { message = "伺服器發生錯誤" });
            }
        }
        
        [HttpPost("ChangePwd")]//更改密碼
        [Authorize]//需要驗證身份才能更改密碼
        public IActionResult ChangePwd([FromBody] ChangePwdDto ChangePwdData)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                                      .SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
                return BadRequest(new
                {
                    message = "資料格式錯誤",
                    errors = errorMessages
                });
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var token = _UserDbContext.LoginCheck(email, ChangePwdData.oldPassword);
            if (token == null)
            {
                return Unauthorized(new { message = "密碼錯誤" });
            }
            _UserDbContext.ChangePwd(email,ChangePwdData.newPassword);
            return Ok(new
            {
                message = "密碼更改成功"
            });
        }


        [HttpGet("EmailValidate")]//驗證信箱
        public IActionResult EmailValidate([FromQuery] int userid, [FromQuery] string auth_code)
        {
            string result = _UserDbContext.EmailValidate(auth_code, userid);

            if (result == "驗證成功")
            {
                return Redirect("http://localhost:5500/registerVerifySuccess.html");
            }
            else
            {
                return Redirect("http://localhost:5500/verifyfail.html");
            }
        }

        [HttpGet("ResetPasswordValidate")]//驗證重設密碼的連結 
        public IActionResult ResetPasswordValidate([FromQuery] int userid, [FromQuery] string auth_code)
        {

            bool isValid = _UserDbContext.ValidateResetToken(userid, auth_code);

            if (isValid)
            {
                return Redirect($"http://localhost:5500/resetPwdVerifySuccess.html?uid={userid}&code={auth_code}");
            }
            else
            {
                return Redirect("http://localhost:5500/verifyfail.html");
            }
        }

        [HttpPost("ForgotPassword")]//忘記密碼
        public IActionResult ForgotPassword([FromBody] ForgotPwdRequestDto request)
        {
            if (string.IsNullOrEmpty(request.email))
            {
                return BadRequest(new { message = "請輸入電子信箱" });
            }

            if (_UserDbContext.EmailCheck(request.email))
            {
                return BadRequest(new { message = "此信箱未註冊" });
            }

            string token = _UserDbContext.GenerateResetToken(request.email);
            var newUser = _UserDbContext.GetDataByEmail(request.email);
            int uid = newUser.uid;

            _mailService.SendResetPasswordMail(request.email, token ,uid);

            return Ok(new { message = "重設密碼信件已寄出" });
        }

        [HttpPost("ResetPassword")]//重設密碼
        public IActionResult ResetPassword([FromBody] Userforgetpassword resetData)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "資料格式錯誤", errors = errorMessages });
            }

            if (resetData.New_password != resetData.confirm_password)
            {
                return BadRequest(new { message = "兩次輸入的密碼不一致" });
            }
            bool success = _UserDbContext.ResetPasswordWithToken(resetData);//去DB驗證
            if (success)
            {
                return Ok(new { message = "密碼更改成功" });
            }
            else
            {
                return BadRequest(new { message = "驗證碼錯誤或連結已失效" });
            }
        }
        [HttpGet("Member")]//取得使用者資料
        [Authorize]
        public IActionResult GetProfile()
        {
             var uidClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst("sub")?.Value;
            if (uidClaim == null) return Unauthorized(new { message = "無效的憑證" });

            int uid = int.Parse(uidClaim);

            var profile = _UserDbContext.GetUserProfile(uid);
            if (profile == null) return NotFound(new { message = "找不到使用者資料" });

            return Ok(profile);
        }

        [HttpPut("Member")]//更新使用者資料
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var uidClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                 ?? User.FindFirst("sub")?.Value;
            if (uidClaim == null)
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            int uid = int.Parse(uidClaim);

            var resultMessage = _UserDbContext.UpdateUserProfile(uid, dto);

            if (resultMessage == "暱稱更新成功" || resultMessage.Contains("信箱已更改"))
            {
                return Ok(new { success = true, message = resultMessage });
            }
            else
            {
                return BadRequest(new { success = false, message = resultMessage });
            }
        }


        [HttpGet("Members")]//取得所有使用者資料，僅限admin角色
        [Authorize(Roles = "admin")]
        public IActionResult Members()
        {
            var profiles = _UserDbContext.GetUserProfiles();

            return Ok(profiles);
        }

        [HttpPut("Admin/Member")]//更新使用者角色，僅限admin角色
        public IActionResult UpdateRole([FromBody] UpdateRoleDto dto)
        {
            var uidClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                 ?? User.FindFirst("sub")?.Value;
            if (uidClaim == null)
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            var resultMessage = _UserDbContext.UpdateRole(dto);

            if (resultMessage == "更改成功")
            {
                return Ok(new { success = true, message = resultMessage });
            }
            else
            {
                return BadRequest(new { success = false, message = resultMessage });
            }
        }
        [HttpGet("total-count")]//取得總使用者人數，僅限admin角色
        public async Task<IActionResult> GetTotalUsersCount()
        {
            try
            {
                int count = await _UserDbContext.GetTotalUsersCountAsync();
                return Ok(new
                {
                    Success = true,
                    TotalUsers = count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "獲取總人數失敗" });
            }
        }
    }
}

