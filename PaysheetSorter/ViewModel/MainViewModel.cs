using CsvHelper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PaysheetSorter.Classes;
using PaysheetSorter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace PaysheetSorter.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            Users = new ObservableCollection<UserData>();
            Clasiffiers = new ObservableCollection<Classifier>();
            SystemLog = new ObservableCollection<string>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                Users.Add(new UserData("User1", "user@usermail"));
                Users.Add(new UserData("User2", "user2@usermail"));
                for (int pagina = 0; pagina < 20; pagina++)
                {
                    Clasiffiers.Add(new Classifier("pagina" + pagina.ToString(), new UserData("patron"+ pagina.ToString(), "email" + pagina.ToString() +"@email.com")));
                }
                

            }
        }

        #region Variables
        Microsoft.Win32.OpenFileDialog _pdfPicker;
        #endregion

        #region BindableProperties
        /// <summary>
        /// The <see cref="PaysheetPdf" /> property's name.
        /// </summary>
        public const string PaysheetPdfPropertyName = "PaysheetPdf";

        private string _paysheetPdf = String.Empty;

        /// <summary>
        /// Sets and gets the PaysheetPdf property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PaysheetPdf
        {
            get
            {
                return _paysheetPdf;
            }

            set
            {
                if (_paysheetPdf == value)
                {
                    return;
                }

                _paysheetPdf = value;
                RaisePropertyChanged(PaysheetPdfPropertyName);
            }
        }
        /// <summary>
        /// Usuarios del sistema
        /// </summary>
        public ObservableCollection<UserData> Users { get; set; }
        public ObservableCollection<Classifier> Clasiffiers { get; set; }
        public static  ObservableCollection<String> SystemLog { get; set; }

        /// <summary>
        /// The <see cref="UsersCsvFile" /> property's name.
        /// </summary>
        public const string UsersCsvFilePropertyName = "UsersCsvFile";

        private string _usersCsvFile = "";

        /// <summary>
        /// Sets and gets the UsersCsvFile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UsersCsvFile
        {
            get
            {
                return _usersCsvFile;
            }

            set
            {
                if (_usersCsvFile == value)
                {
                    return;
                }

                _usersCsvFile = value;
                RaisePropertyChanged(UsersCsvFilePropertyName);
            }
        }

        #endregion

        #region RelayCommands

        private RelayCommand<string> _filePickerCommand;

        /// <summary>
        /// Gets the PDFFilePickerCommand.
        /// </summary>
        public RelayCommand<string> PDFFilePickerCommand
        {
            get
            {
                return _filePickerCommand
                    ?? (_filePickerCommand = new RelayCommand<string>(
                    p =>
                    {
                        Microsoft.Win32.OpenFileDialog openPicker = new Microsoft.Win32.OpenFileDialog();
                        openPicker.DefaultExt = ".pdf";
                        openPicker.Filter = "Nominas en PDF|*.pdf";
                        // Display the OpenFileDialog by calling ShowDialog method
                        Nullable<bool> result = openPicker.ShowDialog();
                        PaysheetPdf = openPicker.FileName;
                        // Dividimos los pdfs automaticamente
                        List<string> sheets = new List<string>();
                        PaysheetManipulator.SplitPdfs(PaysheetPdf, sheets);

                        // Clasificamos los pdfs
                        foreach (string currentSheet in sheets)
                        {
                            UserData user = PaysheetManipulator.ClasifySheet(currentSheet, Users);
                            if (user != null)
                            {
                                Clasiffiers.Add(new Classifier(currentSheet, user));
                            }
                        }

                    }));
            }
        }
        
        private RelayCommand _CSVUserFilePicker;

        /// <summary>
        /// Gets the CsvUSerFilePickerCommand.
        /// </summary>
        public RelayCommand CsvUSerFilePickerCommand
        {
            get
            {
                return _CSVUserFilePicker
                    ?? (_CSVUserFilePicker = new RelayCommand(
                    () =>
                    {
                        Microsoft.Win32.OpenFileDialog openPicker = new Microsoft.Win32.OpenFileDialog();
                        openPicker.DefaultExt = ".csv";
                        openPicker.Filter = "Usuarios  en CSV|*.csv";
                        Nullable<bool> result = openPicker.ShowDialog();
                        UsersCsvFile = openPicker.FileName;
                        List<UserData> users = new List<UserData>(ReadUsersFromCsv(UsersCsvFile));
                        foreach (UserData currentUser in users)
                        {
                            Users.Add(currentUser);
                        }

                    }));
            }
        }


        #endregion


        #region Funciones

        IEnumerable<UserData> ReadUsersFromCsv(string csvPath)
        {
            List<UserData> _users = new List<UserData>();
            CsvReader csv = new CsvReader(File.OpenText(csvPath));
            while (csv.Read())
            {
                _users.Add(new UserData(csv.GetField<string>(0), csv.GetField<string>(1)));
            }
            return _users;
        }

        #endregion

        #region StaticMethods

        // No se si los metodos estaticos en esta clase son adecuados

        public static void AddLogEntry(string logMessage)
        {
            SystemLog.Add(logMessage);
        }


        #endregion

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}