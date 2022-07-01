using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace shopapp.webui.EmailServices
{
    public class SmtpEmailSender : IEmailSender
    {
        public string _host ;
        public int _port;
        public bool _enabledSSL ;
        public string _username ;
        public string _password ;

        public SmtpEmailSender(string host, int port, bool enabledSSL, string username, string password)
        {
            this._host = host;
            this._port = port;
            this._enabledSSL = enabledSSL;
            this._username =username;
            this._password = password;
            
        }
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(this._host,this._port)
            {
                UseDefaultCredentials=false,
                Credentials = new NetworkCredential(_username,_password),
                EnableSsl = this._enabledSSL,
            };
          
            return client.SendMailAsync(
                new MailMessage(this._username , email , subject, htmlMessage){
                    IsBodyHtml = true
                }
            );
        }
    }
}