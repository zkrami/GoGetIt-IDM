using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Classes
{
    class FileDownloaderWriter
    {

        Stream stream;

        
        public FileDownloaderWriter(Stream stream)
        {
            this.stream = stream; 
        }

        public void Write(FileDownloader fileDownloader)
        {
            BinaryFormatter serializer = new BinaryFormatter();            
            serializer.Serialize(stream, fileDownloader);

        }
        public void Write(ICollection<FileDownloader> filesDownloader)
        {
            foreach (var fileDownloader in filesDownloader)
                Write(fileDownloader); 
        }
        public void Close()
        {
            stream.Close(); 
        }
        public void Dispose()
        {
            stream.Dispose(); 
        }

    }
}
