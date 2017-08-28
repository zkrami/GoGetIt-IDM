using IDM;
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
using static IDM.Classes.UrlListener;

namespace GoGetIt
{
    /// <summary>
    /// Interaction logic for UrlCatchWindow.xaml
    /// </summary>
    /// 
            
    
    public partial class UrlCatchWindow : Window
    {
        MainWindow parent;
        string url; 
        public UrlCatchWindow(MainWindow parent , UrlOnCaptureEventArgs eventArgs)
        {
            InitializeComponent();

            this.url = eventArgs.url; 
            urlTextBox.Text = url;
            fileSize.Text = AppHelper.FormatFileSize(eventArgs.size);
            this.fileIcon.Source = AppHelper.GetIcon(this.url, false, false); 
            this.parent = parent; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { // download
            parent.ProccessUrl(url);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        { // download later 
            
            parent.CreateDownloader(url); 
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        { // cancel 
            this.Close();
        }
    }
}
