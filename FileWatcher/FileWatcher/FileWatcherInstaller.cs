using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Configuration;

namespace FileWatcher
{
    [RunInstaller(true)]
    public partial class FileWatcherInstaller : Installer
    {
        public FileWatcherInstaller()
        {
            InitializeComponent();
        }

        //我們得override掉原來的Install()方法
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
           // System.Diagnostics.Debugger.Break();
            //取得使用者輸入的參數值(這邊的Parameter名稱很重要喔，後面的Custom Action還要用到)
            string WatchForder = Context.Parameters["WatchForder"];
            string MailServer = Context.Parameters["MailServer"];
            string MailUserId = Context.Parameters["MailUserId"];
            string MailUserPsd = Context.Parameters["MailUserPsd"];
            string WarningEmail = Context.Parameters["WarningEmail"];
            string WarningSubject = Context.Parameters["WarningSubject"];
            string nowatchPath = Context.Parameters["nowatchPath"];
            string MailSender = Context.Parameters["MailSender"];
            string Interval = Context.Parameters["Interval"];
            //尋找安裝路徑中的App.config檔
            var exeConfigurationFileMap = new ExeConfigurationFileMap();
            exeConfigurationFileMap.ExeConfigFilename = Context.Parameters["assemblypath"] + ".config";
            var config = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            //System.Configuration.Configuration config =
            //   ConfigurationManager.OpenExeConfiguration(
            //       ConfigurationUserLevel.None);

            //將參數值寫回App.config檔



            SetConfigValue("WatchForder", WatchForder, config);
            SetConfigValue("MailServer", MailServer, config);
            SetConfigValue("MailUserId", MailUserId, config);
            SetConfigValue("MailUserPsd", MailUserPsd, config);
            SetConfigValue("WarningEmail", WarningEmail, config);
            SetConfigValue("WarningSubject", WarningSubject, config);
            SetConfigValue("nowatchPath", nowatchPath, config);
            SetConfigValue("MailSender", MailSender, config);
            SetConfigValue("Interval", Interval, config);

            config.Save(ConfigurationSaveMode.Modified);
        }
        private void SetConfigValue(string configKey , string configValue,Configuration conf)
        {
              if (conf.AppSettings.Settings[configKey] == null)
                conf.AppSettings.Settings.Add(configKey, configValue);
            else
                conf.AppSettings.Settings[configKey].Value = configValue;
        }
    }
}
