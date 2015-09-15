using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;


namespace FileWatcher
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private void InvokeMethod(object sender, RenamedEventArgs e)
        {
            InvokeMethod(sender, (FileSystemEventArgs)e, e.OldFullPath);
        }
        private void InvokeMethod(object sender, FileSystemEventArgs e)
        {
            InvokeMethod(sender, e, "");
        }
        private void InvokeMethod(object sender, FileSystemEventArgs e, string OldFullPath)
        {
            

            string dir = Path.GetDirectoryName(e.FullPath).ToLower();
            string nowatchPaths = System.Configuration.ConfigurationSettings.AppSettings["nowatchPath"];
            if (nowatchPaths != "")
            {
                foreach (string nowatchPath in nowatchPaths.ToLower().Split(";|,".ToCharArray()))
                {
                    if (dir.StartsWith(nowatchPath.TrimEnd('\\')))
                        return;
                }
            }

            string ChangeType = "";
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    ChangeType = "檔案變更";
                    break;
                case WatcherChangeTypes.Deleted:
                    ChangeType = "檔案刪除";
                    break;
                case WatcherChangeTypes.Created:
                    ChangeType = "新增檔案";
                    break;
                case WatcherChangeTypes.Renamed:
                    ChangeType = "重新命名";
                    break;
                default:
                    ChangeType = e.ChangeType.ToString();
                    break;
            }

            string mailContent = "";
            if (e.ChangeType == WatcherChangeTypes.Renamed)
                mailContent = String.Format("{0}{1} -- {2} --> {3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss - "), ChangeType, OldFullPath, e.FullPath);
            else
                mailContent = String.Format(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss - ") + ChangeType + " -- " + e.FullPath);

            tr.setMsg(mailContent);



        }

        WatcherTimer tr = new WatcherTimer();
        Watcher w1 ;
        protected override void OnStart(string[] args)
        {
            changeMethodDelegate changeWatchmethod = new changeMethodDelegate(InvokeMethod);
            renameMethodDelegate renameWatchmethod = new renameMethodDelegate(InvokeMethod);

            w1 = new Watcher(System.Configuration.ConfigurationSettings.AppSettings["WatchForder"]//Settings1.Default.WatchForder
                , System.Configuration.ConfigurationSettings.AppSettings["FileFilter"]//Settings1.Default.FileFilter
                , changeWatchmethod
                , renameWatchmethod);
            w1.StartWatch();


            tr.Start();


        }

        protected override void OnStop()
        {
            w1.StopWatch();
            tr.Stop();
        }
    }

 
}
