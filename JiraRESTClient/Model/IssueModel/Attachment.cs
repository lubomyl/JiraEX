using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class Attachment
    {
        private string _size;

        public string Id { get; set; }
        public string Filename { get; set; }
        public DateTime Created { get; set; }

        public string Size
        {
            get
            {
                if (Int32.Parse(_size) >= 1024*1024)
                {
                    return Int32.Parse(_size) / (1024*1024) + "MB";
                }
                else if(Int32.Parse(_size) >= 1024)
                {
                    return Int32.Parse(_size) / 1024 + "kB";
                } else
                {
                    return Int32.Parse(_size)  + "B";
                }
            }
            set
            {
                this._size = value;
            }
        }

        public string Content { get; set; }
    }
}
