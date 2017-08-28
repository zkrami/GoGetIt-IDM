using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
namespace IDM.Classes
{
    class FooStream : Stream
    {
        public override bool CanRead
        {
            get
            {
                return true; 
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return 0;
            }
        }

        public override long Position
        {
            get
            {
                return 0;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
        
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0; 
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0; 
        }

        public override void SetLength(long value)
        {
          
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            
        }
    }
}
