using System.Windows;

namespace WpfStartup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region AppProperties

        #region MainWindow

        //Application wide access to the main window. 
        //Do Not Alter
        public static MainWindow AppMainWindow
        {
            get;
            set;
        }

        void App_Startup(object sender, StartupEventArgs e)
        { 
		    this.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;           
            MainWindow mainWindow = new MainWindow();
            App.AppMainWindow = mainWindow;
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }

        public const System.Int16 FADEDURATION_LONG = 3;
        public const System.Int16 FADEDURATION_SHORT = 1;

        #endregion

        #endregion
    }
}
