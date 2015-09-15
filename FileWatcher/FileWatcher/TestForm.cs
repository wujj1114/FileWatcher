using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;



namespace FileWatcher
{
    public partial class TestForm : Form
    {
        public TestForm()
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
                foreach (string nowatchPath in nowatchPaths.ToLower().Split(";,|".ToCharArray()))
                {
                    if (dir.StartsWith(nowatchPath.TrimEnd('\\')))
                        return;
                }
            }

            string ChangeType = "";
            string mailContent = "";
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Renamed:
                    ChangeType = "重新命名";
                    break;
                case WatcherChangeTypes.Changed:
                    ChangeType = "檔案變更";
                    break;
                case WatcherChangeTypes.Deleted:
                    ChangeType = "檔案刪除";
                    break;
                case WatcherChangeTypes.Created:
                    ChangeType = "新增檔案";
                    break;
                default:
                    ChangeType = e.ChangeType.ToString();
                    break;
            }


            if (e.ChangeType == WatcherChangeTypes.Renamed)
                mailContent = String.Format("{0}{1} -- {2} --> {3}",DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss - ") , ChangeType  ,OldFullPath , e.FullPath);
            else
                mailContent = String.Format(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss - ") + ChangeType + " -- " + e.FullPath);

            tr.setMsg(mailContent);
        }



        Watcher w1;
        WatcherTimer tr = new WatcherTimer();
        private void button1_Click(object sender, EventArgs e)
        {
            changeMethodDelegate changeWatchmethod = new changeMethodDelegate(InvokeMethod);
            renameMethodDelegate renameWatchmethod = new renameMethodDelegate(InvokeMethod);
            w1 = new Watcher(System.Configuration.ConfigurationSettings.AppSettings["WatchForder"]//Settings1.Default.WatchForder
                , System.Configuration.ConfigurationSettings.AppSettings["FileFilter"]//Settings1.Default.FileFilter
                , changeWatchmethod
                , renameWatchmethod);
            w1.StartWatch();

            WatcherTimerDelegate writelog = new WatcherTimerDelegate(log);
            tr.setOutMsg = writelog;
            tr.Start();


        }
        private void log(string arglog)
        {
            if (base.InvokeRequired)
            {
                try
                {
                    WatcherTimerDelegate logmethod = new WatcherTimerDelegate(this.log);
                    Invoke(logmethod, new object[] { arglog });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                   
                }

            }
            else
                richTextBox1.AppendText(arglog);
        }

        
    }
}
