using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IDM.Classes
{
   
        public class UrlListener
        {
            const int PORT = 9898;
            Socket server;

        public class UrlOnCaptureEventArgs
        {
            public string url;
            public long size;
            public UrlOnCaptureEventArgs(string url , long size)
            {
                this.size = size;
                this.url = url;  
            } 
        }
            public delegate void OnUrlCaptureDelegate(UrlOnCaptureEventArgs eventArgs);
            public event OnUrlCaptureDelegate OnUrlCapture;
            public UrlListener()
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            public void Start()
            {
                // only accept from 127.0.0.1
                server.Bind(new IPEndPoint(IPAddress.Loopback, 9898));
                server.Listen(0);
                server.BeginAccept(accpetCallBack, null);

            }
            [STAThread]
            void accpetCallBack(IAsyncResult ar)
            {
                Socket client = server.EndAccept(ar);
                byte[] buffer = new byte[1024 * 100];
                client.Receive(buffer);
                string httpHeaders = "HTTP/1.1" + "\r\n";
                httpHeaders += "Cache-Control: no-cache" + "\r\n";
                httpHeaders += "Access-Control-Allow-Origin: *";
                httpHeaders += "\r\n\r\n";
            string t = System.Text.Encoding.UTF8.GetString(buffer); 
            byte[] byteHttpHeaders = System.Text.Encoding.UTF8.GetBytes(httpHeaders);
            client.BeginSend(byteHttpHeaders, 0, byteHttpHeaders.Length, SocketFlags.None, null, null); 

            string recived = Encoding.Default.GetString(buffer);


                // get url 
                int index = recived.IndexOf("GGI=");
                index += 4;
                StringBuilder sb = new StringBuilder();
                while (index < recived.Length &&  recived[index] != '&' )
                {
                    sb.Append(recived[index]);
                    index++;

                }
                string finalUrl = sb.ToString();

                sb = new StringBuilder();
                index = recived.IndexOf("GGIsize=");
                index += 8;
                while(index < recived.Length &&  recived[index] != '&')
                {
                    sb.Append(recived[index]);
                index++; 
                }
            long size = 0;
            long.TryParse(sb.ToString(), out size);

                Uri uri;
                if (Uri.TryCreate(finalUrl ,UriKind.Absolute , out uri) && OnUrlCapture != null)
                    OnUrlCapture(new UrlOnCaptureEventArgs(finalUrl , size));
                client.Close();

                client.Dispose();
                server.BeginAccept(accpetCallBack, null);


            }
        
    }
}
