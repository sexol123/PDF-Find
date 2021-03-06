using System;
using System.Diagnostics;
using System.IO;
using Model.Entities;
using PdfSharp.Pdf;

namespace Model
{
    public class ReportDataModel
    {
        #region Static

        private static string GetReportName(string filePath) => new PdfDocument(filePath).Info.Title;

        private static void CheckPdfBy(string filePath)
        {
            if (Path.GetExtension(filePath) == "pdf")
                // todo locale    // todo exception type
                throw new ArgumentException("File must be .pdf only", nameof(filePath));

            if (!File.Exists(filePath))
                // todo locale
                throw new FileNotFoundException("Can't find file", filePath);
        }

        #endregion

        #region Properties

        private PrinterConfigurationDataBase DataBase { get; }

        public IApplicationConfigurator ApplicationConfigurator { get; }

        #endregion

        public ReportDataModel(IApplicationConfigurator applicationConfigurator, PrinterConfigurationDataBase dataBase)
        {
            if (applicationConfigurator == null) throw new ArgumentNullException(nameof(applicationConfigurator));
            if (dataBase == null) throw new ArgumentNullException(nameof(dataBase));

            ApplicationConfigurator = applicationConfigurator;
            DataBase = dataBase;
        }

        #region Methods

        /// <summary>
        ///     opens file in prf reader
        /// </summary>
        public void OpenInReader(string filePath)
        {
            CheckPdfBy(filePath);

            var process = new Process
            {
                StartInfo = {FileName = ApplicationConfigurator.ReaderPath, Arguments = Path.GetFullPath(filePath)}
            };
            if (!process.Start())
                throw new ArgumentException("Can't open pdf reader");

            DataBase.FileOpeningDatas.Add(new FileReadingData
            {
                FileName = Path.GetFileNameWithoutExtension(filePath),
                Opening = DateTime.Now
            });
            DataBase.SaveChanges();
        }

        /// <summary>
        ///     delegates file to printer with <paramref name="configuration" />
        /// </summary>
        public void OpenForPrint(string filePath, ReportConfiguration configuration)
        {
            CheckPdfBy(filePath);

            configuration.AddFile(Path.GetFileNameWithoutExtension(filePath));
            DataBase.SaveChanges();
            throw new NotImplementedException("delegate to printer");
        }

        /// <summary>
        ///     Tryes to find report by it's name. Returns <c>null</c> if no one;
        /// </summary>
        public ReportConfiguration FindReport(string filePath)
        {
            CheckPdfBy(filePath);

            return DataBase.FindReport(GetReportName(filePath));
        }

        /// <summary>
        ///     Tryes to find report group by it's name. Return LAST of the list or <c>null</c> if no one;
        /// </summary>
        public GroupConfiguration FindGroup(string filePath)
        {
            CheckPdfBy(filePath);

            return DataBase.FindGroup(GetReportName(filePath));
        }

        #endregion
    }
}