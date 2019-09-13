using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using System.Configuration;

namespace ViewModel
{
    public class RegistryApplicationConfigurator : IApplicationConfigurator
    {
        #region Fields

        private string _language;
        private string _readerPath;
        private string _dataBaseConnectionString;

        private readonly string _regKey;

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public string Language
        {
            get { return _language; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    // todo locale
                    throw new ArgumentException("Argument is null or whitespace", nameof(value));
                if (_language == value) return;
                _language = value;
                Properties.Settings.Default.DefaultCulture = _language;
                OnPropertyChanged();
            }
        }

        public string ReaderPath
        {
            get { return _readerPath; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    // todo locale
                    throw new ArgumentException("Argument is null or whitespace", nameof(value));
                if (_readerPath == value)
                    return;
                if (!File.Exists(value))
                    throw new FileNotFoundException("Reader wasn't found", value); // todo locale
                if (Path.GetExtension(value).Equals("exe", StringComparison.OrdinalIgnoreCase))
                    // todo locale            // todo exception type
                    throw new ArgumentException("Reader must be *.exe application", nameof(value));

                // use fullPath or name if program writen in Path?

                _readerPath = Path.GetFullPath(value);
                OnPropertyChanged();
            }
        }

        public string DataBaseConnectionString
        {
            get { return _dataBaseConnectionString; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    // todo locale
                    throw new ArgumentException("Argument is null or whitespace", nameof(value));
                _dataBaseConnectionString = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        #endregion

        public RegistryApplicationConfigurator()
        {
            SaveCommand = new Prism.Commands.DelegateCommand<ICommand>(Save);

            var key = new AppSettingsReader().GetValue("RegKey", typeof(string)).ToString();

            _regKey = key;
            
            var currentUser = Registry.CurrentUser;

            using (var pdfFindKey = currentUser.OpenSubKey(_regKey))
            {
                if (pdfFindKey != null)
                {

                    Language = pdfFindKey.GetValue("language").ToString();
                    ReaderPath = pdfFindKey.GetValue("readerPath").ToString();
                    DataBaseConnectionString = pdfFindKey.GetValue("dbConnectionString").ToString();
                }
                else
                {
                    _language = "en";
                    _readerPath = "";
                    _dataBaseConnectionString = "";
                }
            }
        }

        #region Methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private void Save(ICommand command)
        {
            if (String.IsNullOrWhiteSpace(Language) &&
                String.IsNullOrWhiteSpace(ReaderPath) &&
                String.IsNullOrWhiteSpace(DataBaseConnectionString)) {
                command.Execute(false);
                return;
            }

            try {
                var currentUser = Registry.CurrentUser;
                var pdfFindKey = currentUser.OpenSubKey(_regKey, true) ?? currentUser.CreateSubKey(_regKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (pdfFindKey == null) return;
                pdfFindKey.SetValue("language", Language);
                pdfFindKey.SetValue("readerPath", ReaderPath);
                pdfFindKey.SetValue("dbConnectionString", DataBaseConnectionString);
                pdfFindKey.Close();

                command.Execute(true);
            }
            catch {
                command.Execute(false);
            }
        }

        #endregion
    }
}