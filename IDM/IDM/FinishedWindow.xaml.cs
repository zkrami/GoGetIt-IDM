using IDM.Classes;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for FinishedWindow.xaml
    /// </summary>
    public partial class FinishedWindow : Window
    {
        public FileDownloader downloader;   
        public FinishedWindow(FileDownloader downloader)
        {
            InitializeComponent();
            this.downloader = downloader;


            this.DataContext = downloader;
             
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // open click event 
            string path = downloader.FilePath;
            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("The file has been deleted or removed");

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Open Folder Event 

            string path = downloader.FilePath;
            string directory = Directory.GetParent(path).FullName; 
            if (Directory.Exists(directory))
                System.Diagnostics.Process.Start(directory);
            else
                MessageBox.Show("The folder has been deleted or removed");

            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Close Event 
            this.Close();
        }
    }
}
