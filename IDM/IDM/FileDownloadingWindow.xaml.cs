using IDM.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IDM
{
    /// <summary>
    /// Interaction logic for FileDownloadingWindow.xaml
    /// </summary>
    public partial class FileDownloadingWindow : Window
    {

        public FileDownloader fileDownloader;  
        public FileDownloadingWindow(FileDownloader fileDownloader)
        {

            this.fileDownloader = fileDownloader;
            this.DataContext = fileDownloader;
            fileDownloader.ClearPauseEvents();
            fileDownloader.OnPaused += FileDownloader_OnPaused;


            fileDownloader.ClearFinishDownloadEvents();
            fileDownloader.OnFinshDownload += FileDownloader_OnFinshDownload;
            InitializeComponent();
            fileDownloader.window = this; 
            if(fileDownloader.State  == FileDownloader.FileDownloadState.Paused
                || fileDownloader.State == FileDownloader.FileDownloadState.Failed)
                pauseResumeBtn.Content = "Resume";

        }

        private void FileDownloader_OnFinshDownload(FileDownloader downloader)
        {
       
            pauseResumeBtn.IsEnabled = false;
        }

        private void FileDownloader_OnPaused(FileDownloader downloader)
        {
            this.Dispatcher.Invoke(() =>
            {
                pauseResumeBtn.IsEnabled = true;
                pauseResumeBtn.Content = "Resume"; 
            });

        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {


            if(fileDownloader.State == FileDownloader.FileDownloadState.Paused
                || fileDownloader.State == FileDownloader.FileDownloadState.Failed)
            {
             
                fileDownloader.Resume();
                pauseResumeBtn.IsEnabled = false;

                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    this.Dispatcher.Invoke(() =>
                    {
                        if(this.fileDownloader.State != FileDownloader.FileDownloadState.Completed
                        && this.fileDownloader.State != FileDownloader.FileDownloadState.Collecting)
                        pauseResumeBtn.IsEnabled = true; 
                    });
                }); 
                pauseResumeBtn.Content = "Pause";


            }
            else
            {

             
                fileDownloader.Pause(); 
                pauseResumeBtn.IsEnabled = false;
                
                

            }
            
        }
        int p = 0; 

        private void window_Closed(object sender, EventArgs e)
        {
            fileDownloader.window = null; 
        }
    }
}
