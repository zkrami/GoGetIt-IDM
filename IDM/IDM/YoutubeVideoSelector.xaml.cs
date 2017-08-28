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
using YoutubeExtractor;

namespace IDM
{
    /// <summary>
    /// Interaction logic for YoutubeVideoSelector.xaml
    /// </summary>
    public partial class YoutubeVideoSelector : Window
    {


        public string urlVideo;
        public VideoInfo video;
        public MainWindow parent; 
        public YoutubeVideoSelector(string urlVideo , MainWindow parent )
        {
            InitializeComponent();
            this.urlVideo = urlVideo;
            this.parent = parent;
            this.urlTextBox.Text = urlVideo; 
            this.Loaded += YoutubeVideoSelector_Loaded;
            this.resolutionComboBox.SelectionChanged += ResolutionComboBox_SelectionChanged;
            this.Topmost = false;
            this.Topmost = true; 
        }

        private void ResolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            video = resolutionComboBox.SelectedItem as VideoInfo; 
        }

        private void YoutubeVideoSelector_Loaded(object sender, RoutedEventArgs e)
        {
            this.RetriveInfos(); 
        }

        public async void  RetriveInfos()
        {


            await Task.Run(() =>
            {
                IEnumerable<VideoInfo> videoInfos;
                try
                {
                    videoInfos = DownloadUrlResolver.GetDownloadUrls(this.urlVideo, false);
                    this.Dispatcher.Invoke(() =>
                    {
                        resolutionComboBox.Items.Clear(); 
                        foreach (VideoInfo vid in videoInfos)
                        {
                            
                            resolutionComboBox.Items.Add(vid);
                        }
                        if(resolutionComboBox.Items.Count > 0 )
                        resolutionComboBox.SelectedIndex = 0; 
                    });
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });
                    MessageBox.Show("Please Check Your Connection And Try Again Later");
                    
                    
                    AppHelper.Log(ex.Message);

                }
            }); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            video = null;
            this.Close(); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (video == null) return; 
            parent.DownloadVideo(video);
            this.Close(); 
        }
    }
}
