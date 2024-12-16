using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Web_Bán_Hàng.Models;

namespace Web_Bán_Hàng.Areas.Repository
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true, 
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("truong.pn.64cntt@ntu.edu.vn", "qlbnglxsmymbvwrl")
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("truong.pn.64cntt@ntu.edu.vn"),
				Subject = subject,
				Body = htmlMessage,
				IsBodyHtml = true, 
			};

			mailMessage.To.Add(email);

			return client.SendMailAsync(mailMessage);
		}
	}
}
