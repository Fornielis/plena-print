using System.Net;
using System.Net.Mail;

namespace MinhaPre
{
    public class FuncaoEmail
    {
        public void EnviarEmailComercial(OS os)
        {
            // INSTANCIAL DATA SISTEMA
            var dataSistema = new Data.Sistema();

            // OBTEM E-MAIL SISTEMA - SYS
            var emailSistemaLista = dataSistema.Email_DadosPorTipo("SYS");
            var emailSistema = new Email();

            foreach (var item in emailSistemaLista)
            {
                emailSistema.E_mail = item.E_mail;
                emailSistema.Senha = item.Senha;
                emailSistema.Porta = item.Porta;
            }

            // OBTEM EMAILS COMERCIAL - COM
            var emailComercial = dataSistema.Email_DadosPorTipo("COM");

            // CONFIGURAÇÃO CORPO DO E-MAIL
            var mailMessage = new MailMessage();

            // PREENCHE EMAIL PARA ENVIO COMERCIAL
            foreach (var email in emailComercial)
            {
                mailMessage.To.Add(new MailAddress(email.E_mail));
            }

            mailMessage.From = new MailAddress(emailSistema.E_mail);
            mailMessage.Subject = os.Numero + " - Orçamento OK";

            // OBTENDO TEMPLATE HTML
            //WebClient wc = new WebClient();
            //wc.Encoding = System.Text.Encoding.UTF8;
            //string sTemplate = wc.DownloadString("http://www.cbsa.com.br/exemplos/template.html");
            //mailMessage.IsBodyHtml = true;
            //mailMessage.Body = sTemplate;

            mailMessage.IsBodyHtml = true;
            mailMessage.Body =
            "<hr/>" +
                "<br/>Orçamento: " + "<b>" + os.Numero + "</b> <br/>" +
                "<br/>Cliente: " + "<b>" + os.Cliente + "</b> <br/>" +
                "<br/>Material: " + "<b>" + os.Material + "</b> <br/> <br/>" +
            "<hr/>";

            // CONFIGURAÇÃO PARA ENVIO
            var smtpCliente = new SmtpClient("smtp.kinghost.net", emailSistema.Porta);
            smtpCliente.Credentials = new NetworkCredential(emailSistema.E_mail, emailSistema.Senha);
            smtpCliente.EnableSsl = false;
            smtpCliente.Send(mailMessage);
        }
    }
}