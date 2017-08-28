using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Classes
{
    class FileDownloaderReader
    {

        Stream stream; 
        public FileDownloaderReader(Stream stream)
        {
            this.stream = stream; 
        }

        bool CanRead
        {
            get { return stream.CanRead;  }
        }

        public FileDownloader ReadNext()
        {

            BinaryFormatter serializer = new BinaryFormatter();            
            return (FileDownloader)serializer.Deserialize(stream);
        }
        public ObservableCollection<FileDownloader> ReadAll()
        {

            ObservableCollection<FileDownloader> fileDownloaders = new ObservableCollection<FileDownloader>(); 
            while(stream.Position != stream.Length)
            {
                fileDownloaders.Add(ReadNext()); 
            }
            return fileDownloaders;
        }
    }
}
