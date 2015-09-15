using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileWatcher
{
    delegate void changeMethodDelegate(object sender, FileSystemEventArgs e);
    delegate void renameMethodDelegate(object sender, RenamedEventArgs e);
    public class Watcher
    {

        public string Directory { get; set; }
        public string Filter { get; set; }
        public bool IncludeSubdirectories { get; set; }


        private Delegate _renameMethod;
        private Delegate _changeMethod;

        public Delegate ChangeMethod
        {
            get { return _changeMethod; }
            set { _changeMethod = value; }
        }

        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

        public Watcher(string directory, string filter, Delegate invokeMethod, Delegate renameMethod)
        {
            this._renameMethod = renameMethod;
            this._changeMethod = invokeMethod;
            this.Directory = directory;
            this.Filter = Filter;
            this.IncludeSubdirectories = true;
        }


        public void StartWatch()
        {


            fileSystemWatcher.Filter = this.Filter;
            fileSystemWatcher.Path = this.Directory;
            fileSystemWatcher.IncludeSubdirectories = this.IncludeSubdirectories;
            fileSystemWatcher.InternalBufferSize = 16384;
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Changed +=
                new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Created +=
                new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Deleted +=
                new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Renamed +=
                new RenamedEventHandler(fileSystemWatcher_Rename);
        }
  
        public void StopWatch()
        {
            fileSystemWatcher.Changed -=
              new FileSystemEventHandler(fileSystemWatcher_Changed);
        }



        void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_changeMethod != null)
            {
               // fileSystemWatcher.EnableRaisingEvents = false;  //記得先停止動監控，以免觸發多次

                _changeMethod.DynamicInvoke(sender, e);
              //  fileSystemWatcher.EnableRaisingEvents = true; //執行完畢後，請再重新啟動
            }
        }
        void fileSystemWatcher_Rename(object sender, RenamedEventArgs e)
        {
            if (_renameMethod != null)
            {
                // fileSystemWatcher.EnableRaisingEvents = false;  //記得先停止動監控，以免觸發多次
                _renameMethod.DynamicInvoke(sender, e);
                //  fileSystemWatcher.EnableRaisingEvents = true; //執行完畢後，請再重新啟動
            }
        }
    }
}
