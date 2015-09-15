using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace FileWatcher
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            this.AfterInstall += new InstallEventHandler(ProjectInstaller_AfterInstall);

        }
        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            //為了要讓程式完成安裝後就直接啟用
            System.ServiceProcess.ServiceController sc = new System.ServiceProcess.ServiceController(serviceInstaller1.ServiceName);
            if (sc != null)
                sc.Start();
        }

        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
