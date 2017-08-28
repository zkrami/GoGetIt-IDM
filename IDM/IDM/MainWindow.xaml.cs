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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IDM.Classes;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Threading;
using System.Text.RegularExpressions;
using YoutubeExtractor;

namespace IDM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        UrlListener urlListener; 
        public void ShowWindow(object sender, EventArgs args)
        {
            this.ShowInTaskbar = true;
            
            this.Show();
            this.Topmost = false;

            this.Topmost = true;
            
           
            
            this.WindowState = WindowState.Normal;
            
        }
        public MainWindow()
        {
            
            InitializeComponent();
            
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/GoGetIt;component/res/icon.ico")).Stream); 
            ni.Visible = true;
            ni.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            ni.ContextMenuStrip.Items.Add("Show");
            ni.ContextMenuStrip.Items.Add("Exit");
            
            ni.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
            ni.DoubleClick += ShowWindow; 
                

            RefreshGird();

            Style rowStyle = downloadsGrid.RowStyle;


            rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseRightButtonDownEvent,
                                    new MouseButtonEventHandler(Row_RightClick)));

            urlListener = new UrlListener();
            urlListener.Start();
            urlListener.OnUrlCapture += UrlListener_OnUrlCapture;


            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private void ContextMenuStrip_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
       
            if(e.ClickedItem.Text == "Exit")
            {

            
                if (FileDownloader.downloadsList.Any(u => u.State == FileDownloader.FileDownloadState.Receiving || u.State == FileDownloader.FileDownloadState.Receiving))
                {

                    MessageBox.Show("Please Stop ALl Downloads First");
                }
                else
                {
                    save_file();

                    Application.Current.Shutdown(0);
                }
            }
            else
            {
                ShowWindow(this, new EventArgs()); 
            }
            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            load_file();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //closing

            e.Cancel = true;
            this.Hide();
    
            this.ShowInTaskbar = false;
            this.WindowState = WindowState.Minimized; 
  
            
        }

       
        private void UrlListener_OnUrlCapture(UrlListener.UrlOnCaptureEventArgs args)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (AppHelper.YoutubeRegex.IsMatch(args.url))
                {
                    ProccessUrl(args.url);
                }
                else
                {
                    Window t = new GoGetIt.UrlCatchWindow(this, args);
                    t.Show();
                    t.Topmost = false;
                    t.Topmost = true;
                }
            });
        }

        private void Row_RightClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            DataGridRow row = sender as DataGridRow;
            int index = row.GetIndex();

            downloadsGrid.SelectedIndex = index;
            ContextMenu cm = this.FindResource("menu") as ContextMenu;
            cm.PlacementTarget = row;
            cm.IsOpen = true;

            //  cm.IsOpen = true;


        }
       public void RefreshGird()
        {
            downloadsGrid.ItemsSource = FileDownloader.downloadsList;
        }
        public void AttachEvents(FileDownloader downloader)
        {
            downloader.OnFileReady += Downloader_OnFileReady;
            downloader.OnFailedConnection += Downloader_OnFailedDownload;
        }
        public void ProccessUrl(string url)
        {


            if (AppHelper.FileUrlRegex.IsMatch(url)) return; 

            Uri res;
            if (!Uri.TryCreate(url, UriKind.Absolute, out res))
            {
                MessageBox.Show("Please Enter A Valid Url ");
                return;
            }

            if (AppHelper.YoutubeRegex.IsMatch(url))
            {
                //downloader = new YoutubeDownloader(new Uri(url));
                YoutubeVideoSelector win = new YoutubeVideoSelector(url , this);
                win.Show();
            }else
            {
                DownloadUrl(url); 
            }

        }
        public FileDownloader CreateDownloader(string url)
        {

            FileDownloader downloader = null;

            downloader = new FileDownloader(new Uri(url));
            AttachEvents(downloader);
            downloader.AddToList();
            return downloader; 
        }

        public FileDownloader CreateDownloader(VideoInfo video)
        {

            FileDownloader downloader = null;
            downloader = new YoutubeDownloader(video);
            AttachEvents(downloader);
            downloader.AddToList();
            return downloader;
        }

        public async void DownloadVideo(VideoInfo video)
        {




            FileDownloader downloader = CreateDownloader(video);

            if (await downloader.DownloadAsync())
            {
                Window window = new FileDownloadingWindow(downloader);
                window.Show();
                window.Topmost = false;
                window.Topmost = true;

            }

        }
        public async void DownloadUrl(string url)
        {

            
           

            FileDownloader downloader = CreateDownloader(url); 

            if (await downloader.DownloadAsync())
            {
                Window window = new FileDownloadingWindow(downloader);
                window.Show();                           
                window.Topmost = false;
                window.Topmost = true;

            }

        }
        private  void button_Click(object sender, RoutedEventArgs e)
        {



            ProccessUrl(textBox.Text);


        }

        private void Downloader_OnFileReady(FileDownloader downloader)
        {
            if (downloader.window != null) downloader.window.Close();
            (new FinishedWindow(downloader)).ShowDialog();
        }

        private void Downloader_OnFailedDownload(FileDownloader downloader)
        {
            if (downloader.window != null)
                downloader.window.pauseResumeBtn.Content = "Resume";

            MessageBox.Show("Please check your connection and try again later ");
           
        }

        private void Downloader_OnFinshDownload(FileDownloader downloader)
        {
      
        }

        void update_Brogress(FileDownloader downloader)
        {


        }

        private void save_file()
        {
          
                    FileDownloaderWriter writer = new FileDownloaderWriter(new FileStream("GGI.data", FileMode.Create, FileAccess.Write));
                    writer.Write(FileDownloader.downloadsList);

            writer.Close();





        }

        private void load_file()
        {



            if (!File.Exists("GGI.data")) return; 
            FileDownloaderReader reader = new FileDownloaderReader(File.OpenRead("GGI.data"));

            FileDownloader.downloadsList = reader.ReadAll();

            foreach (var download in FileDownloader.downloadsList)
            {
                AttachEvents(download);
                download.AttachPartsEvents();
            }



            RefreshGird();


        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            FileDownloader downloader = downloadsGrid.SelectedValue as FileDownloader;

            if (downloader.window != null)
            {
                downloader.window.Show();
            }
            else if (downloader.State != FileDownloader.FileDownloadState.Completed)
            {
                Window window = new FileDownloadingWindow(downloader);
                window.Show();
            }

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            FileDownloader downloader = downloadsGrid.SelectedValue as FileDownloader;

            if (downloader.window != null)
            {
                downloader.window.Close();
            }
            downloader.Pause();

        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            FileDownloader downloader = downloadsGrid.SelectedValue as FileDownloader;

            if (downloader.window != null)
            {
                downloader.window.Close();
            }
            if(downloader.State == FileDownloader.FileDownloadState.Receiving)
            {
                downloader.OnPaused += (FileDownloader t) => {

                    t.DeleteCollectorFolder();
                    FileDownloader.downloadsList.Remove(t);
                    this.Dispatcher.Invoke(() =>
                    {
                        this.RefreshGird();
                    });
                    
                }; 
                downloader.Pause();
            }else
            {
                downloader.DeleteCollectorFolder();
                FileDownloader.downloadsList.Remove(downloader);
                this.Dispatcher.Invoke(() =>
                {
                    this.RefreshGird();
                });
            }
            


        }
        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            
            FileDownloader downloader = downloadsGrid.SelectedValue as FileDownloader;
            if (downloader.State != FileDownloader.FileDownloadState.Paused
                && downloader.State != FileDownloader.FileDownloadState.Failed) return; 
            if (downloader.window != null)
            {
                downloader.window.pauseResumeBtn.Content = "Pause";  
            }

            downloader.Resume();


        }
    }
}
