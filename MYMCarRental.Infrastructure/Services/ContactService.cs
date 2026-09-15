using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MYMCarRental.Application.DTOs.Contact;
using MYMCarRental.Application.Interfaces;
using MYMCarRental.Infrastructure.Email;

namespace MYMCarRental.Infrastructure.Services;

public sealed class ContactService : IContactService
{
    private readonly EmailSettings _emailSettings;

    public ContactService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendMessageAsync(
        SendContactMessageDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _emailSettings.FromName,
                _emailSettings.FromEmail));

        email.To.Add(
            MailboxAddress.Parse(_emailSettings.CompanyEmail));

        email.ReplyTo.Add(
            MailboxAddress.Parse(dto.Email));

        email.Subject = $"New Contact Message - {dto.FullName}";

        var body = new BodyBuilder
        {
            HtmlBody = BuildHtmlBody(dto)
        };

        email.Body = body.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        var secureSocketOption = _emailSettings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTls;

        await smtp.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            secureSocketOption,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _emailSettings.Username,
            _emailSettings.Password,
            cancellationToken);

        await smtp.SendAsync(
            email,
            cancellationToken);

        await smtp.DisconnectAsync(
            true,
            cancellationToken);
    }


private static string BuildHtmlBody(
    SendContactMessageDto dto)
    {
        var fullName =
            System.Net.WebUtility.HtmlEncode(dto.FullName);

        var phone =
            System.Net.WebUtility.HtmlEncode(dto.Phone);

        var email =
            System.Net.WebUtility.HtmlEncode(dto.Email);

        var message =
            System.Net.WebUtility.HtmlEncode(dto.Message)
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");

        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <title>New Contact Message</title>
        </head>

        <body style="
            margin: 0;
            padding: 40px 20px;
            background-color: #f3f3f3;
            font-family: Arial, Helvetica, sans-serif;
            color: #1a1a1a;
        ">

            <div style="
                max-width: 680px;
                margin: 0 auto;
                background-color: #ffffff;
                border-radius: 14px;
                overflow: hidden;
                box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08);
            ">

                <!-- HEADER -->
                <div style="
                    background-color: #0b0d10;
                    padding: 32px 35px;
                    text-align: center;
                    border-bottom: 3px solid #c8a96b;
                ">

                    <div style="
                        font-size: 26px;
                        font-weight: 700;
                        letter-spacing: 1.5px;
                        color: #ffffff;
                    ">
                        MYM
                    </div>

                    <div style="
                        margin-top: 5px;
                        font-size: 12px;
                        font-weight: 600;
                        letter-spacing: 3px;
                        text-transform: uppercase;
                        color: #c8a96b;
                    ">
                        CAR RENTAL
                    </div>

                </div>

                <!-- CONTENT -->
                <div style="
                    padding: 35px;
                ">

                    <div style="
                        margin-bottom: 28px;
                    ">

                        <div style="
                            display: inline-block;
                            padding: 7px 12px;
                            background-color: #f7f1e6;
                            color: #a8894f;
                            border-radius: 20px;
                            font-size: 11px;
                            font-weight: 700;
                            letter-spacing: 1px;
                            text-transform: uppercase;
                        ">
                            Contact Request
                        </div>

                        <h1 style="
                            margin: 16px 0 8px;
                            font-size: 25px;
                            line-height: 1.3;
                            color: #0b0d10;
                        ">
                            New Contact Message
                        </h1>

                        <p style="
                            margin: 0;
                            font-size: 14px;
                            line-height: 1.7;
                            color: #777777;
                        ">
                            A new message has been submitted through the
                            MYM Car Rental website.
                        </p>

                    </div>

                    <!-- CUSTOMER DETAILS -->
                    <div style="
                        border: 1px solid #e8e8e8;
                        border-radius: 12px;
                        overflow: hidden;
                        margin-bottom: 25px;
                    ">

                        <div style="
                            padding: 15px 18px;
                            background-color: #0b0d10;
                            color: #c8a96b;
                            font-size: 12px;
                            font-weight: 700;
                            letter-spacing: 1px;
                            text-transform: uppercase;
                        ">
                            Customer Details
                        </div>

                        <div style="padding: 20px;">

                            <table
                                width="100%"
                                cellpadding="0"
                                cellspacing="0"
                                style="
                                    border-collapse: collapse;
                                    font-size: 14px;
                                "
                            >

                                <tr>
                                    <td style="
                                        width: 110px;
                                        padding: 8px 0;
                                        color: #888888;
                                        font-weight: 600;
                                    ">
                                        Full Name
                                    </td>

                                    <td style="
                                        padding: 8px 0;
                                        color: #111111;
                                        font-weight: 600;
                                    ">
                                        {fullName}
                                    </td>
                                </tr>

                                <tr>
                                    <td style="
                                        padding: 8px 0;
                                        color: #888888;
                                        font-weight: 600;
                                    ">
                                        Phone
                                    </td>

                                    <td style="
                                        padding: 8px 0;
                                        color: #111111;
                                    ">
                                        {phone}
                                    </td>
                                </tr>

                                <tr>
                                    <td style="
                                        padding: 8px 0;
                                        color: #888888;
                                        font-weight: 600;
                                    ">
                                        Email
                                    </td>

                                    <td style="
                                        padding: 8px 0;
                                    ">
                                        <a
                                            href="mailto:{email}"
                                            style="
                                                color: #a8894f;
                                                text-decoration: none;
                                                font-weight: 600;
                                            "
                                        >
                                            {email}
                                        </a>
                                    </td>
                                </tr>

                            </table>

                        </div>
                    </div>

                    <!-- MESSAGE -->
                    <div style="
                        border: 1px solid #e8e8e8;
                        border-radius: 12px;
                        overflow: hidden;
                    ">

                        <div style="
                            padding: 15px 18px;
                            background-color: #f8f8f8;
                            border-bottom: 1px solid #e8e8e8;
                            color: #0b0d10;
                            font-size: 12px;
                            font-weight: 700;
                            letter-spacing: 1px;
                            text-transform: uppercase;
                        ">
                            Message
                        </div>

                        <div style="
                            padding: 22px;
                            background-color: #ffffff;
                            font-size: 15px;
                            line-height: 1.8;
                            color: #444444;
                        ">
                            {message}
                        </div>

                    </div>

                </div>

                <!-- FOOTER -->
                <div style="
                    padding: 25px 35px;
                    background-color: #0b0d10;
                    text-align: center;
                ">

                    <div style="
                        color: #ffffff;
                        font-size: 13px;
                        font-weight: 600;
                    ">
                        MYM Car Rental
                    </div>

                    <div style="
                        margin-top: 6px;
                        color: #999999;
                        font-size: 11px;
                        line-height: 1.6;
                    ">
                        This message was sent from the MYM Car Rental website.
                    </div>

                    <div style="
                        margin-top: 12px;
                        height: 1px;
                        background-color: #2a2d31;
                    "></div>

                    <div style="
                        margin-top: 12px;
                        color: #c8a96b;
                        font-size: 10px;
                        letter-spacing: 1px;
                        text-transform: uppercase;
                    ">
                        Luxury. Comfort. Freedom.
                    </div>

                </div>

            </div>

        </body>
        </html>
        """;
    }

}