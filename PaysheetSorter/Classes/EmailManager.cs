using PaysheetSorter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PaysheetSorter.Classes
{
    public class EmailManager
    {
        public static int sendFileByEmail(string filePath, string smtpServer, string senderEmailAddress , string senderPassword, string DestiantionAddress )
        {
            SmtpClient smtpClient = new SmtpClient(smtpServer, 25);

            smtpClient.Credentials = new System.Net.NetworkCredential(senderEmailAddress, senderPassword);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(senderEmailAddress, "PDFAnalizer");
            mail.To.Add(new MailAddress(To));
            mail.Attachments.Add(new Attachment(filePath));

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                MainViewModel.AddLogEntry("Error al enviar el email");
            }
            

            return 0;
        }
    }
}
