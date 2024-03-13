using System.Net;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Identity.Client;

var password = File.ReadAllText("password");
const string login = "skiba_al@itstep.edu.az";

#region SMTP

//
// var password = File.ReadAllText("password");
// var login = "skiba_al@itstep.edu.az";
//
// using var smtpClient = new SmtpClient();
//
// await smtpClient.ConnectAsync("smtp-mail.outlook.com", 587);
// await smtpClient.AuthenticateAsync(login, password);
//
// var message = new MimeMessage
// {
//     Sender = new MailboxAddress(string.Empty, login),
//     Subject = "SMTP Protocol (text+image)"
// };
//
// message.To.Add(new MailboxAddress(string.Empty, "alesk_le73@itstep.edu.az"));
// message.To.Add(new MailboxAddress(string.Empty, "hagve_rq27@itstep.edu.az"));
// message.To.Add(new MailboxAddress(string.Empty, "gamdu_yo52@itstep.edu.az"));
//
//
// // Send file
// // using var file = File.OpenRead("dog.jpg");
// //
// // message.Body = new MimePart("image/jpg")
// // {
// //     Content = new MimeContent(file)
// // };
//
// // Send file and text
// using var file = File.OpenRead("dog.png");
// Multipart entity = new Multipart();
//
// entity.Add(new TextPart
// {
//     Text = "Snoop Dog art xDDDD"
// });
//
// entity.Add(new MimePart("image/png")
// {
//     Content = new MimeContent(file),
//     FileName = "dog.png"
// });
//
// entity.Add(new TextPart("html")
// {
//     Text = "<ul style=\"color:red;\"><li>Item1</li><li>Item2</li><li>Item3</li></ul>"
// });
//
// message.Body = entity;
//
// await smtpClient.SendAsync(message);
// await smtpClient.DisconnectAsync(true);

#endregion

#region IMAP

// var options = new PublicClientApplicationOptions
// {
// };
//
// var clientApplication = PublicClientApplicationBuilder.CreateWithApplicationOptions(options).Build();
//
// var scopes = new string[]
// {
//     "email",
//     "https://outlook.office.com/IMAP.AccessAsUser.All", // Only needed for IMAP
//     //"https://outlook.office.com/POP.AccessAsUser.All",  // Only needed for POP
//     //"https://outlook.office.com/SMTP.Send", // Only needed for SMTP
// };
//
// var authToken = await clientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
// var oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);
//
// using var client = new ImapClient();
//
// await client.ConnectAsync("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);
// await client.AuthenticateAsync(oauth2);
// await client.DisconnectAsync(true);

#endregion