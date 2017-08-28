using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExtractor;
namespace IDM.Classes
{

    [Serializable]
    class YoutubeDownloader : FileDownloader
    {
        [NonSerialized]
        VideoInfo video; 
        public YoutubeDownloader(VideoInfo video ,  int partsCount = 8 ) : base(new Uri(video.DownloadUrl), partsCount) { this.video = video;  }
        public override async Task<bool> ConnectAsync()
        {

            return await Task<bool>.Run<bool>(async () =>
            {
             
                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }
                FileName = video.Title + video.VideoExtension;
                
                this.FilePath = FileDownloader.GetFilePath(FileName); 
                return await base.ConnectAsync();
               
            }); 



                
            
            
        }
    }
}
