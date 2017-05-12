using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaysheetSorter.Classes
{
    public class Classifier
    {
        public string PdfSheet { get; set; }
        public string SheetFileName { get; set; }
        public UserData User { get; set; }


        public Classifier(string pdfSheet, UserData user)
        {
            PdfSheet = pdfSheet;
            SheetFileName = pdfSheet.Split('\\').Last();
            User = user;
        }


        private RelayCommand _sendEmailCommand;

        /// <summary>
        /// Gets the SendEmailCommand.
        /// </summary>
        public RelayCommand SendEmailCommand
        {
            get
            {
                return _sendEmailCommand
                    ?? (_sendEmailCommand = new RelayCommand(
                    () =>
                    {
                        EmailManager.sendFileByEmail(PdfSheet, smtp_server, from, pass,User.Email);
                    }));
            }
        }

    }
}
