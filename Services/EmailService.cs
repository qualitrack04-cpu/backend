using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace QualiTrack.Services;

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string otp);
    Task SendRegistrationOtpAsync(string toEmail, string otp);
}

public class EmailService(IConfiguration config) : IEmailService
{
    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(config["Email:From"]!));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        smtp.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(config["Email:From"]!, config["Email:Password"]!);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    // Untuk forgot password
    public async Task SendOtpAsync(string toEmail, string otp)
    {
        await SendEmailAsync(toEmail,
            "QualiTrack - Kode OTP Reset Password",
            $"""
            <div style="font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;">
                <h2 style="color: #1a3a5c;">QualiTrack</h2>
                <p>Kamu menerima email ini karena ada permintaan reset password.</p>
                <p>Gunakan kode OTP berikut:</p>
                <div style="background: #f0f4f8; padding: 20px; text-align: center; border-radius: 8px;">
                    <h1 style="color: #1a3a5c; letter-spacing: 8px;">{otp}</h1>
                </div>
                <p>Kode berlaku selama <strong>5 menit</strong>.</p>
                <p>Jika kamu tidak meminta reset password, abaikan email ini.</p>
                <br>
                <p style="color: #888;">QualiTrack Team</p>
            </div>
            """);
    }

    // Untuk registrasi
    public async Task SendRegistrationOtpAsync(string toEmail, string otp)
    {
        await SendEmailAsync(toEmail,
            "QualiTrack - Verifikasi Email",
            $"""
            <div style="font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;">
                <h2 style="color: #1a3a5c;">QualiTrack</h2>
                <p>Selamat datang di QualiTrack! 🎉</p>
                <p>Gunakan kode OTP berikut untuk verifikasi email kamu:</p>
                <div style="background: #f0f4f8; padding: 20px; text-align: center; border-radius: 8px;">
                    <h1 style="color: #1a3a5c; letter-spacing: 8px;">{otp}</h1>
                </div>
                <p>Kode berlaku selama <strong>5 menit</strong>.</p>
                <p>Jika kamu tidak mendaftar di QualiTrack, abaikan email ini.</p>
                <br>
                <p style="color: #888;">QualiTrack Team</p>
            </div>
            """);
    }
}