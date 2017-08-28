using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Text.RegularExpressions;

namespace IDM.Classes
{


    [Serializable]
    public class FileDownloader : INotifyPropertyChanged
    {
        [NonSerialized]
        public FileDownloadingWindow window = null;


        [NonSerialized]
          Timer timer; 
        static public ObservableCollection<FileDownloader> downloadsList = new ObservableCollection<FileDownloader>();

        public void AddToList()
        {


            downloadsList.Add(this);


        }
        public static string FormatFileSize(double value)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double size = value;
            int unit = 0;

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return String.Format("{0:0.#} {1}", size, units[unit]);

        }



        public enum FileDownloadState { SendingHeader = 0, Paused, Receiving, Error, NotFound, Completed, Collecting, Failed }

        string FormatState(FileDownloadState state)
        {
            switch (state)
            {
                case FileDownloadState.SendingHeader:
                    return "Sending Header...";
                case FileDownloadState.Receiving:
                    return "Receiving...";
                case FileDownloadState.NotFound:
                    return "Not Found";
                case FileDownloadState.Completed:
                    return "Completed";
                case FileDownloadState.Error:
                    return "Error";

                case FileDownloadState.Collecting:
                    return "Collecting Parts...";

                case FileDownloadState.Paused:
                    return "Paused";
                case FileDownloadState.Failed:
                    return "Failed";


                default:
                    return "Not impletented";


            }

        }

        FileDownloadState state = FileDownloadState.Paused;


        public FileDownloadState State
        {
            protected set
            {
                if (state == value) return; 
                state = value;
                OnPropertyChange("State");
                OnPropertyChange("StateFormated");
                if (value == FileDownloadState.Paused)
                {
                    Close();
                    pausing = false;
                    if (OnPaused != null)
                    {
                        OnPaused(this);
                    }
                    
                }
                if(value == FileDownloadState.Failed)
                {
                    Close();
                    if (OnFailedConnection != null)
                        OnFailedConnection(this); 
                }

            }
            get { return state; }
        }
        public string StateFormated { get { return FormatState(state); } }



        public long downloaded = 0;


        public long Downloaded
        {
            private set
            {
                downloaded = value;
                OnPropertyChange("Downloaded");
                OnPropertyChange("DownloadedFormatted");

            }
            get { return downloaded; }
        }
        public long Reamining
        {
            get { return FileSize - Downloaded; }
        }
        public string DownloadedFormatted
        {
            get
            {
                return FormatFileSize(Downloaded) + " " + String.Format(FileSize == 0 ? "0%" : (Downloaded / (double)FileSize).ToString("0.##%"));
            }
        }



        public string FormatedFileSize { get { return FormatFileSize(FileSize); } }
        public long fileSize = 0;
       
        public ImageSource Img
        {
            get


            {
                return AppHelper.GetIcon(fileName, false, false); 
                
            }
        }
        public long FileSize
        {
            protected set
            {
                fileSize = value;
                OnPropertyChange("FileSize");
                OnPropertyChange("FormatedFileSize");


            }
            get { return fileSize; }
        }

        // Download Name 


        public string fileName;


        public string FileName
        {
            protected set
            {
                fileName = value;
                fileName = RemoveIllegal(fileName);
                OnPropertyChange("FileName");
                OnPropertyChange("Img");

            }
            get { return fileName; }
        }

        static string RemoveIllegal(string t)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach(char c in invalid)
            {
                t = t.Replace(c.ToString(), ""); 
            }
            return t; 
        }
        // Download Path 
        public string filePath ;
        public string FilePath { protected set
            {
                filePath = value;
               
            }
            get
            {
                return filePath;
            }

            }


        DateTime lastTry; 
        public DateTime LastTry
        {
            private set
            {
                lastTry = value;
                OnPropertyChange("LastTry"); 
            }
            get { return lastTry; }
        }

        // Collector Folder 

        public string CollectorFolder { private set; get; }

        [NonSerialized]
        protected HttpWebResponse webResponse;
        [NonSerialized]
        protected HttpWebRequest webRequest;


        [NonSerialized]

        protected CancellationTokenSource tokenSource;
        [NonSerialized]
        protected CancellationToken connectToken;
        public int PartsCount { get; private set; }

        public Uri Url { get; protected set; }


        ObservableCollection<PartFileDownloader> partsFileDownloader = new ObservableCollection<PartFileDownloader>();
        public ObservableCollection<PartFileDownloader> PartsFileDownloader { private set { partsFileDownloader = value; } get { return partsFileDownloader; } }

        int partsFinished = 0;

        [NonSerialized]
        FilesCollector fileCollector;



        static string downloadPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        static string downloadGoGetItPath = Path.Combine(downloadPath, "GoGetIt");

        static FileDownloader()
        {
            // create the download folder 
            if (!Directory.Exists(downloadGoGetItPath))
                Directory.CreateDirectory(downloadGoGetItPath);

        }



        public delegate void UpdateDownloadHandler(FileDownloader downloader);
        [field: NonSerialized]
        public event UpdateDownloadHandler OnUpdateDownload;


        public delegate void FinishDownloadHandler(FileDownloader downloader);
        [field: NonSerialized]
        public event FinishDownloadHandler OnFinshDownload;


        public delegate void OnFileReadHandler(FileDownloader downloader);
        [field: NonSerialized]
        public event OnFileReadHandler OnFileReady;




        public delegate void OnPausedHandler(FileDownloader downloader);
        [field: NonSerialized]
        public event OnPausedHandler OnPaused;


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;






        public delegate void FailedDownloadHandler(FileDownloader downloader);
        [field: NonSerialized]
        public event FailedDownloadHandler OnFailedConnection;
        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public FileDownloader(Uri url, int partsCount = 8)
        {
            this.Url = url;
            PartsCount = partsCount;

            this.FilePath = GetFilePath(Url.AbsolutePath);
            this.FileName = Path.GetFileName(FilePath);

        }
        public FileDownloader() { } // for serialiazation 
        protected static string GetFilePath(string url)
        {



            string nameExt = Path.GetFileName(url);

            
            string name = Path.GetFileNameWithoutExtension(nameExt);
            name = RemoveIllegal(name); 
            string ext = Path.GetExtension(nameExt);
            //string name = Path.GetFileNameWithoutExtension(url.AbsolutePath); 

            string downloadPath = Path.Combine(downloadGoGetItPath, nameExt);
            int i = 1;
            while (File.Exists(downloadPath) || downloadsList.Any(t => t.FilePath == downloadPath))
            {
                downloadPath = Path.Combine(downloadGoGetItPath, name + i + ext);


                i++;
            }

            return downloadPath;


        }

        static string GetCollectorFolder(string name)
        {
            string collectorFolder = Path.Combine(FilesCollector.CollectorFolder, name);

            int i = 1;
            while (Directory.Exists(collectorFolder))
            {
                collectorFolder = Path.Combine(FilesCollector.CollectorFolder, name + i);
                i++;
            }
            return collectorFolder;

        }
        public void DeleteCollectorFolder()
        {
            try
            {
                if (Directory.Exists(CollectorFolder))
                    Directory.Delete(CollectorFolder, true);
            }
            catch (Exception ex) { }
        }




        // return false if failed connection 

         public virtual async Task<bool> ConnectAsync()
        {


            // Create a request to the file we are downloading           
            webRequest = (HttpWebRequest)WebRequest.Create(Url);

            webRequest.ServicePoint.ConnectionLimit = 64;

            // Set default authentication for retrieving the file
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            // Retrieve the response from the server
            tokenSource = new CancellationTokenSource();
            connectToken = tokenSource.Token;

            if (connectToken.IsCancellationRequested)
            {
                State = FileDownloadState.Paused;
                return false;
            }


            try
            {
                webResponse = await webRequest.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException ex)
            {

                if (connectToken.IsCancellationRequested)
                {
                    State = FileDownloadState.Paused;
                    return false;
                }
                State = FileDownloadState.Failed;

                AppHelper.Log(ex.Message);
            }




            if (connectToken.IsCancellationRequested)
            {
                State = FileDownloadState.Paused;

                return false;
            }



            if (State == FileDownloadState.Failed) return false;


            // Ask the server for the file size and store it
            FileSize = webResponse.ContentLength;
            Close();
            return true; 
        }
        public virtual async Task<bool> DownloadAsync()
        {

            LastTry = DateTime.Now;

            State = FileDownloadState.SendingHeader;
            // Get the path and the name for the file (handle the repeated name)

            if (!await ConnectAsync()) return false; 


            long offset = 0;

            // devide the amount between the parts 
            long amountToRead = FileSize / PartsCount;
            // take care of the reamaing amount 
            long firstAmountToRead = amountToRead + (FileSize % PartsCount);

            
            // get folder for the parts files 
            this.CollectorFolder = GetCollectorFolder(FileName);
            Directory.CreateDirectory(this.CollectorFolder);

            State = FileDownloadState.Receiving;
         
            failedPart = 0;


           
            
            StartTimer(); 
            for (int i = 0; i < PartsCount; i++)
            {
                PartFileDownloader part;
                string partPath = Path.Combine(this.CollectorFolder, "part" + i);

                //treat first part differently 
                if (i == 0)
                {
                    part = new PartFileDownloader(this, offset, firstAmountToRead, partPath);
                    offset += firstAmountToRead;
                }
                else
                {
                    part = new PartFileDownloader(this, offset, amountToRead, partPath);
                    offset += amountToRead;
                }

                part.OnUpdateDownload += Part_OnUpdateDownload;
                part.OnFinishDownload += Part_OnFinishDownload;
                part.OnFailedConnection += Part_OnFailedConnection;
                part.DownloadAsync();
                
                part.Number = i + 1;
                part.Index = i;
                
                App.Current.Dispatcher.Invoke((Action) delegate // <--- HERE
                {
                    PartsFileDownloader.Add(part);
                });
            }
            
            return true;
        }

        public void AttachPartsEvents()
        {

            foreach (PartFileDownloader part in PartsFileDownloader)
            {
                part.OnUpdateDownload += Part_OnUpdateDownload;
                part.OnFinishDownload += Part_OnFinishDownload;
                part.OnFailedConnection += Part_OnFailedConnection;
            }
        }
        int failedPart = 0;
        private void Part_OnFailedConnection(PartFileDownloader partFileDownload)
        {

            int t =  Interlocked.Increment(ref failedPart);
            // if all or some of the parts failed 
            if (t + partsFinished == PartsCount)
            {          
                State = FileDownloadState.Failed;
             
            }

        }

        void StartTimer()
        {
            
            timer = new Timer(UpdateTransfer, null, 0, 200);

        }
        protected void Close()
        {
            if (webResponse != null)
            {
                webResponse.Close();
                webResponse.Dispose(); 
            }
            if(webRequest != null)
            webRequest.Abort();
            if(timer != null)
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer = null; 

        }


        // returns whether can pause or not 
        int partsToPaused;
        bool pausing = false;
        public void Pause()
        {
            if (State == FileDownloadState.Collecting || State == FileDownloadState.Completed) return;

            if (State == FileDownloadState.SendingHeader)
            {
                tokenSource.Cancel();
                webRequest.Abort();
             

            }
            else
            {
                pausing = true;
                partsToPaused = PartsCount;
                foreach (PartFileDownloader p in PartsFileDownloader)
                {
                    p.ClearPauseEvents();
                    p.OnPaused += P_OnPaused;
                    p.Pause();
                }
            }
        }
        

        public void ClearPauseEvents()
        {
            if (OnPaused == null) return;
            foreach (Delegate d in OnPaused.GetInvocationList())
                OnPaused -= d as OnPausedHandler;
        }
        public void ClearFinishDownloadEvents()
        {
            if (OnFinshDownload == null) return;
            foreach (Delegate d in OnFinshDownload.GetInvocationList())
                OnFinshDownload -= d as FinishDownloadHandler;
        }

        private void P_OnPaused(PartFileDownloader partFileDownload)
        {
            if (!pausing)
            {
                // partFileDownload.State = PartFileDownloader.PartFileDownloaderState.Failed;
                return;
            }
            int t = Interlocked.Decrement(ref partsToPaused);

            if (t - failedPart == 0)
            {
                State = FileDownloadState.Paused;

            }

        }

        public void Resume()
        {
            LastTry = DateTime.Now;
            failedPart = 0;
            StartTimer();
            // the parts weren't created 
            if (PartsFileDownloader.Count == 0)
            {
                DownloadAsync();
            }
            else
            {
                State = FileDownloadState.Receiving;
                
                foreach (PartFileDownloader p in PartsFileDownloader)
                {
                    p.Resume();
                }
            }
        }


        private async void Part_OnFinishDownload(PartFileDownloader partFileDownload)
        {
            int t = Interlocked.Increment(ref partsFinished);

            if (t == PartsCount)
            {
                // the download has been finished 

                Close();

                fileCollector = new FilesCollector(this, FilePath);
                fileCollector.OnFinishCollect += FileCollector_OnFinishCollect;

                State = FileDownloadState.Collecting;

                fileCollector.Collect();

                OnFinshDownload?.Invoke(this);


            }
        }

        private void FileCollector_OnFinishCollect(FilesCollector sender)
        {

            State = FileDownloadState.Completed;
            OnFileReady?.Invoke(this);
        }


        double transferRate = 0;

        double TransferRate
        {
            set
            {
                transferRate = value;
                OnPropertyChange("TransferRate");
                OnPropertyChange("TransferRateFormated");
            }
            get { return transferRate; }
        }
        public string TransferRateFormated
        {
            get { return FormatFileSize(transferRate) + " / s "; }
        }


        double timeLeft;
        double TimeLeft
        {

            set
            {
                timeLeft = value;
                OnPropertyChange("TimeLeft");
                OnPropertyChange("TimeLeftFormatted");
            }
            get
            {
                return timeLeft;
            }
        }

        public string TimeLeftFormatted
        {
            get { return FormatTime(timeLeft); }
        }

        string FormatTime(double seconds)
        {

            if (double.IsInfinity(seconds)) return "inf";
            if (double.IsNaN(seconds)) return "0";
            TimeSpan time = TimeSpan.MaxValue;
            try
            {
                time = TimeSpan.FromSeconds(seconds);
            }
            catch { }

            if (time.Days != 0)
            {
                return string.Format("{0} Days , {1} Hours", time.Days, time.Hours);
            }
            if (time.Hours != 0)
            {
                return string.Format("{0} Hours , {1} Minutes", time.Hours, time.Minutes);
            }
            if (time.Minutes != 0)
            {
                return string.Format("{0} Minutes , {1} Seconds", time.Minutes, time.Seconds);
            }
            return string.Format("{0} Seconds ", time.Seconds);
        }
          void UpdateTransfer(object state)
          {
              TransferRate = TransferRate +   (Downloaded - lastBytesDownloaded) / (DateTime.Now - lastDownloadedTime).TotalSeconds;
                TransferRate /= 2; 
              // v = x / t 
              // t = x / v 
              if(TransferRate != 0 )
              TimeLeft = Reamining / TransferRate;

              lastDownloadedTime = DateTime.Now;
              lastBytesDownloaded = Downloaded;
          }
        [NonSerialized]
        DateTime lastDownloadedTime = DateTime.Now;
        [NonSerialized]
        long lastBytesDownloaded = 0;
        private void Part_OnUpdateDownload(PartFileDownloader partFileDownload, int bytesDownloaded)
        {
            Downloaded += bytesDownloaded;
            
            OnUpdateDownload?.Invoke(this);
        }
    }

   
}
