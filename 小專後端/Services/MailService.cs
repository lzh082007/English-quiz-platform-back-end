using System.Net;
using System.Net.Mail;

namespace 小專後端.Services
{
    public class MailService
    {
        private readonly string g_Email = "lzh082007@gmail.com";

        private readonly string g_password = "cbqdkyktrtlbtbun";

        private readonly string gmail_mail = "lzh082007@gmail.com";
        public void SendRegisterMail(string nickname, string ToEmail, string authCode,int uid)
        {
            string verifyLink = $"https://localhost:7205/api/User/EmailValidate?userid={uid}&auth_code={authCode}";

            string MailBody = $@"
                英文app註冊。<br>
                  <a href='{verifyLink}'>點我驗證</a><br>
                  <br>";

            using (SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"))
            { 
            //這裡是使用Gmail的SMTP伺服器來發送郵件，首先設定SMTP伺服器的地址和端口，
            // 然後設定認證資訊（使用者名稱和密碼），最後建立一個MailMessage物件來設定郵件的內容，
            // 包括寄件人、收件人、主旨和內容，最後呼叫SmtpClient的Send方法來發送郵件
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(g_Email, g_password);
                SmtpServer.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gmail_mail);
                mail.To.Add(ToEmail);
                mail.Subject = "會員註冊確認信";
                mail.Body = MailBody;
                mail.IsBodyHtml = true;

                SmtpServer.Send(mail);
            }
        }
        public void SendResetPasswordMail(string ToEmail, string token,int uid)
        {
            string resetLink = $"https://localhost:7205/api/User/ResetPasswordValidate?userid={uid}&auth_code={token}";

            string MailBody = $@"
        <h2>忘記密碼重設</h2>
        <a href='{resetLink}'>點我驗證</a><br>
        <br>";

            using (SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"))
            {
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(g_Email, g_password);
                SmtpServer.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gmail_mail);
                mail.To.Add(ToEmail);
                mail.Subject = "密碼重設通知信";
                mail.Body = MailBody;
                mail.IsBodyHtml = true;

                SmtpServer.Send(mail);
            }
        }
        public void SendEmailChangeVerifyMail(string nickname, string ToEmail, string authCode, int uid)
        {
            string verifyLink = $"https://localhost:7205/api/User/EmailValidate?userid={uid}&auth_code={authCode}";

            string MailBody = $@"
        <h2>信箱更改驗證</h2>
        <p>{nickname}</p>
        <a href='{verifyLink}'>點我驗證新信箱</a><br>
        <br>";

            using (SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"))
            {
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new NetworkCredential(g_Email, g_password);
                SmtpServer.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gmail_mail);
                mail.To.Add(ToEmail);
                mail.Subject = "會員信箱更改確認信";
                mail.Body = MailBody;
                mail.IsBodyHtml = true;

                SmtpServer.Send(mail);
            }
        }
    }
}
   
