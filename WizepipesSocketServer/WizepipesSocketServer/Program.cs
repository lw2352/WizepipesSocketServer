using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WizepipesSocketServer
{
    static class Program
    {
        public static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //全局异常捕捉
            Application.ThreadException += Application_ThreadException; //UI线程异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; //多线程异常

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        //UI线程异常
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            //WinformException.FrmBugReport.ShowBug(e.Exception);//执行异常处理
            Log.Debug(e.Exception);
        }

        //多线程异常
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //WinformException.FrmBugReport.ShowBug((Exception)e.ExceptionObject);//执行异常处理
            Log.Debug((Exception)e.ExceptionObject);
        }
    }
}
