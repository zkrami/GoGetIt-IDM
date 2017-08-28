using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDM.Classes
{


    [Serializable]
    public class PartFileDownloader : INotifyPropertyChanged
    {

        public enum PartFileDownloaderState { Connecting = 0, Failed,  Paused, Ignored , Receiving, Error, NotFound, Completed,  Disconnected, Connected, Disconnecting }


        public int Number { set; get; }
        public int Index { set; get; }

        
        string FormatState(PartFileDownloaderState state)
        {
            switch (state)
            {
                case PartFileDownloaderState.Connecting:
                    return "Connecting...";
                case PartFileDownloaderState.Receiving:
                    return "Receiving...";
                case PartFileDownloaderState.NotFound:
                    return "Not Found";
                case PartFileDownloaderState.Completed:
                    return "Completed";
                case PartFileDownloaderState.Error:
                    return "Error";


                case PartFileDownloaderState.Paused:
                    return "Paused";

                case PartFileDownloaderState.Failed:
                    return "Failed";
                default:
                    return "Not impletented";


            }

        }
        PartFileDownloaderState state = PartFileDownloaderState.Disconnected;
        public PartFileDownloaderState State
        {
            set
            {
                if (state == value) return; 
                state = value;
                OnPropertyChange("State");
                OnPropertyChange("StateFormated");
                if(state == PartFileDownloaderState.Paused)
                {
                    this.Close();
                    if (OnPaused != null)
                        OnPaused(this); 
                }
                if(state == PartFileDownloaderState.Failed)
                {
                    this.Close();
                    if (OnFailedConnection != null)
                        OnFailedConnection(this);
                
                }
                if(state == PartFileDownloaderState.Error)
                {
                    this.Close();
                }

            }
            get
            {
                return state;
            }
        }
        public string StateFormated { get { return FormatState(state); } }



        long amountToRead;
        public long AmountToRead
        {
            private set
            {
                OnPropertyChange("AmountToRead");
                OnPropertyChange("AmountToReadFormated");
                amountToRead = value;
            }
            get
            {
                return amountToRead;
            }
        }
        public string AmountToReadFormatted
        {

            get
            {
                return FileDownloader.FormatFileSize(amountToRead);
            }
        }

        long offset;


        long remaining = 0 ;




        long downloaded;
        public long Downloaded
        {
            private set
            {
                downloaded = value;
                OnPropertyChange("Downloaded");
                OnPropertyChange("DownloadedFormated");
            }
            get
            {
                return downloaded;
            }
        }

        public string DownloadedFormated { get { return FileDownloader.FormatFileSize(downloaded) + " " + String.Format(AmountToRead == 0 ? "0%" : (Downloaded / (double)AmountToRead).ToString("0.##%")); } }


        
        public string PartFilePath { get; private set; }
        Uri url;
        //WebClient wc;


        [NonSerialized]
        Stream strLocal;
        [NonSerialized]
        Stream strResponse;
        [NonSerialized]
        FileDownloader parentDownloader;
        [NonSerialized]
        HttpWebRequest webRequest;
        [NonSerialized]
        HttpWebResponse webResponse;
       
        [NonSerialized]
        CancellationTokenSource tokenSource;
        [NonSerialized]
        CancellationToken connectToken , downloadToken; 

        

        // A buffer for storing and writing the data retrieved from the server


        const int bufferLength = 2 * 1024 * 1024;
        [NonSerialized]
        byte[] downBuffer;

        public delegate void OnFinishDownloadHandler(PartFileDownloader partFileDownload);
        [field: NonSerialized]
        public event OnFinishDownloadHandler OnFinishDownload;

        public delegate void OnUpdateDownloadHandler(PartFileDownloader partFileDownload, int bytesDownloaded);
        [field: NonSerialized]
        public event OnUpdateDownloadHandler OnUpdateDownload;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;


        public delegate void OnFailedConnectionHandler(PartFileDownloader partFileDownload);
        [field: NonSerialized]
        public event OnFailedConnectionHandler OnFailedConnection;




        public delegate void OnPausedHandler(PartFileDownloader partFileDownload);
        [field: NonSerialized]
        public event OnPausedHandler OnPaused;
        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        // used for serialization
        public PartFileDownloader() { }


        public PartFileDownloader(FileDownloader parentDownloader, long offset, long amountToRead, string partFilePath)
        {
            this.offset = offset;
            this.AmountToRead = amountToRead;
            this.PartFilePath = partFilePath;
            this.parentDownloader = parentDownloader;
            this.url = parentDownloader.Url;
        }

        public void ClearPauseEvents()
        {
            if (OnPaused == null) return; 
            foreach (Delegate d in OnPaused.GetInvocationList())
                OnPaused -=  d as OnPausedHandler; 
        }
        void Close()
        {

            // Close And Dispose All the resourses   


            if (strResponse != null)
            {
                strResponse.Close();
                strResponse.Dispose();
            }

            if (strLocal != null)
            {
                strLocal.Close();
                strLocal.Dispose();
            }

            if(webResponse != null)
            webResponse.Close();


            if(webRequest != null)
            webRequest.Abort();
            
            downBuffer = null;
        }
        public void Pause()
        {

            // there is nothing to be stopped
            if (State == PartFileDownloaderState.Completed || State == PartFileDownloaderState.Ignored)
            {

                // needed to pause parent 
                if (OnPaused != null) OnPaused(this);
                return;
            }

            tokenSource.Cancel();
            webRequest.Abort();
           


        }
        public async Task ConnectAsync()
        {


             connectToken = tokenSource.Token;
            if (connectToken.IsCancellationRequested)
            {
                State = PartFileDownloaderState.Paused;
                return; 
            }

                State = PartFileDownloaderState.Connecting;
                webRequest = HttpWebRequest.CreateHttp(url);

            // Add the part to download 
             webRequest.AddRange(offset);
           

                // Get Response async 

                
                try
                {
                    webResponse = (HttpWebResponse) await webRequest.GetResponseAsync();
                }
                    catch (WebException ex)
                {

                    if (connectToken.IsCancellationRequested)
                    {
                        State = PartFileDownloaderState.Paused;
                        return;
                    }

                State = PartFileDownloaderState.Failed;
                    AppHelper.Log(ex.Message);
                }
        




            if (connectToken.IsCancellationRequested)
            {
                State = PartFileDownloaderState.Paused;
                return;
            }




            strResponse = webResponse.GetResponseStream();

            State = PartFileDownloaderState.Connected;
          

        }
      
       

      void CompleteDownload()
       {
            State = PartFileDownloaderState.Completed;

            this.Close();

            // Fire Finsihed Event 
            if (OnFinishDownload != null)
                OnFinishDownload(this);
        }
        public async void Resume()
        {
            // if the part is completely downloaded 
            // or it has been taken from another part 
            if (State == PartFileDownloaderState.Completed || State == PartFileDownloaderState.Ignored) return;
            await DownloadAsync(); 
        }
        public async Task DownloadAsync()
        {

            
            tokenSource = new CancellationTokenSource();


            if (AmountToRead == 0)
            {
                CompleteDownload();
                return;
            }

            if (remaining == 0) // (if not resuming ) 
                remaining = AmountToRead;


          
           await ConnectAsync();
            

            // if it's not connected then it's a failure or a pause request  
            if (State != PartFileDownloaderState.Connected) return;

            downloadToken = tokenSource.Token;
            if (downloadToken.IsCancellationRequested)
            {
                State = PartFileDownloaderState.Paused; 
                return;
            }


            // Create a new file stream where we will be saving the data (local drive)
            try
            {
                  strLocal = new FileStream(PartFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                
            }catch(IOException ex)
            {

                AppHelper.Log(ex.Message);
                State = PartFileDownloaderState.Error;
                return;
            }
            
            downBuffer = new byte[bufferLength];

            State = PartFileDownloaderState.Receiving;


            while (remaining != 0)
            {
                        // is paused
                        if (downloadToken.IsCancellationRequested)
                        {
                            State = PartFileDownloaderState.Paused;
                            break;                            
                        }

                int bytesSize = 0;
                try
                {
                    
                    bytesSize =  await strResponse.ReadAsync(downBuffer, 0, (int)Math.Min(remaining, downBuffer.Length)) ;

                }catch(Exception ex)
                {


                    if (connectToken.IsCancellationRequested)
                    {
                        State = PartFileDownloaderState.Paused;
                        return;
                    }

                    State = PartFileDownloaderState.Failed;
                    AppHelper.Log(ex.Message);
               
                    break;
                }
                await strLocal.WriteAsync(downBuffer, 0, bytesSize);

                Downloaded += bytesSize;
                remaining -= bytesSize;
                offset += bytesSize;

                if (OnUpdateDownload != null)
                 OnUpdateDownload(this, bytesSize);



                }



            // free resources 
            if (State == PartFileDownloaderState.Paused || State == PartFileDownloaderState.Failed) return;
            



            if(remaining == 0)
            CompleteDownload(); 




        }





    }

}
