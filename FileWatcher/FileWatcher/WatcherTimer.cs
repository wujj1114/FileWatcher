using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace FileWatcher
{
    delegate void WatcherTimerDelegate(string msg);

 
        //System.Configuration.ConfigurationSettings.AppSettings["KeyName"];

    class WatcherTimer
    {
        Timer _TimersTimer = null;
        StringBuilder _msg = new StringBuilder();
        public WatcherTimer()
        {
            this._TimersTimer = new Timer();
            this._TimersTimer.Interval = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["Interval"]);// Settings1.Default.Interval;
            this._TimersTimer.Elapsed += new System.Timers.ElapsedEventHandler(_Timer_Elapsed);
            
        }
        public void Start()
        {
            this._TimersTimer.Start();
        }
        public void Stop()
        {
            this._TimersTimer.Stop();
        }
        public void setMsg(string msg)
        {
            _msg.AppendLine(msg);
        }
        private void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_msg.Length == 0)
                return;

            string mailContent = _msg.ToString();

            Eventlog(mailContent, EventLogEntryType.Warning);

            FileWatcher.Mail m = new Mail();
            m.Send(
                System.Configuration.ConfigurationSettings.AppSettings["MailServer"],//Settings1.Default.MailServer,
                "",
                "",
                System.Configuration.ConfigurationSettings.AppSettings["MailSender"],//Settings1.Default.MailSender,
                "",
                System.Configuration.ConfigurationSettings.AppSettings["WarningEmail"],//Settings1.Default.WarningEmail,
                "",
                System.Configuration.ConfigurationSettings.AppSettings["WarningSubject"],//Settings1.Default.WarningSubject,
                mailContent.Replace("\r\n", "<br>"),
                null
                );

            OutMsg(mailContent);

            _msg = new StringBuilder();

        }
        private void Eventlog(string Logmsg, EventLogEntryType logtype)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "FileWatcher";
                eventLog.WriteEntry(Logmsg, logtype, 101, 1);
            }
        }

        private Delegate _OutMsg;
        public Delegate setOutMsg
        {
            set { _OutMsg = value; }
        }
        void OutMsg(string msg)
        {
            if (_OutMsg != null)
            {
                _OutMsg.DynamicInvoke(msg);
            }
        }
    }
}
