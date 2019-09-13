
using System.Windows.Input;
using Model;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ViewModel
{
    public class StateReport
   {
       public static StateReport Instance { get; } = new StateReport();

        private ReportConfiguration report;

        public IList<PaperFormat> PaperFormats
        {
            get
            {
                // Will result in a list like {"Tester", "Engineer"}
                return Enum.GetValues(typeof(PaperFormat)).Cast<PaperFormat>().ToList<PaperFormat>();
            }
        }
        
        public ReportConfiguration CurrentReportConfiguration
        {
            get { return report; }
            set
            {
                if (value == null) return;
                report = value;
            }
       }

       public  ICommand SaveCommand { get; private set; }

       private StateReport()
       {
           SaveCommand = new Prism.Commands.DelegateCommand(Save);
       }
       
       private void Save()
       {
           var db = new Model.DataBase(new RegistryApplicationConfigurator().DataBaseConnectionString);
           db.ReportConfigurations.Add(CurrentReportConfiguration);
           db.SaveChanges();
            
       }
    }
}
