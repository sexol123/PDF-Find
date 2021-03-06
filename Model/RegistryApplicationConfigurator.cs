
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Model
{
    public class RegistryApplicationConfigurator : IApplicationConfigurator
    {
        #region Fields

        private string _language;
        private string _readerPath;
        private string _dataBaseConnectionString;

        private string _regKey;

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

        #endregion

        public RegistryApplicationConfigurator(string regKey)
        {
            _regKey = regKey;

            RegistryKey currentUser = Registry.CurrentUser;
            RegistryKey pdfFindKey = currentUser.OpenSubKey(regKey);

            if (pdfFindKey != null) {

                Language = pdfFindKey.GetValue("language").ToString();
                ReaderPath = pdfFindKey.GetValue("readerPath").ToString();
                DataBaseConnectionString = pdfFindKey.GetValue("dbConnectionString").ToString();

                pdfFindKey.Close();
            }
            else {
                _language = "english";
                _readerPath = "";
                _dataBaseConnectionString = "";
            }
        }

        #region Methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Save() {
            RegistryKey currentUser = Registry.CurrentUser;
            RegistryKey pdfFindKey = currentUser.OpenSubKey(_regKey, true) ?? currentUser.CreateSubKey(_regKey, true);

            pdfFindKey.SetValue("language", Language);
            pdfFindKey.SetValue("readerPath", ReaderPath);
            pdfFindKey.SetValue("dbConnectionString", DataBaseConnectionString);
            pdfFindKey.Close();
        }

        #endregion
    }
}