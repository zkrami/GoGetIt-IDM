using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDM.Classes
{
    
    public class FilesCollector
    {


        FileDownloader fileDownloader;

        string filePath;
        int partsToCollect; 

        
        Stream strLocal;  
        static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string collectorFolderName = "GoGetIt"; 
        public static string CollectorFolder { private set; get;  }



        public delegate void OnFinishCollectHandler(FilesCollector sender);
       
        public event OnFinishCollectHandler OnFinishCollect;

        static FilesCollector()
        {
            CollectorFolder = Path.Combine(appData, collectorFolderName);
            if (!Directory.Exists(CollectorFolder))
                Directory.CreateDirectory(CollectorFolder);

        }

        public FilesCollector(FileDownloader fileDownloader, string filePath)
        {
            this.fileDownloader = fileDownloader;
            this.partsToCollect = fileDownloader.PartsCount; 
            this.filePath = filePath;
            strLocal = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);


            
        }
        public async Task Collect()
        {

            

           
            for(int i = 0; i  < partsToCollect; i ++)
            {
               

                FileStream file = File.OpenRead(fileDownloader.PartsFileDownloader[i].PartFilePath); 
                await file.CopyToAsync(strLocal);
                    
                file.Close();
                file.Dispose(); 
            }

             
            strLocal.Close();
            strLocal.Dispose();

            if (OnFinishCollect != null)
                OnFinishCollect(this);


            //Delete  parts files 
            Directory.Delete(fileDownloader.CollectorFolder, true); 



           


            


        }


    }

 
}
