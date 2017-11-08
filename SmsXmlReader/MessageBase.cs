using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public abstract class MessageBase
    {
        public MessageBase() { }


        private string _id;

        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                    _id = ComputeId();

                return _id;
            }
        }

        public DateTime? MessageDate { get; set; }

        public string Address { get; set; }

        public string ContactName { get; set; }

        public DateTime? GetDate(string dateStr)
        {
            long dateLong;

            if (long.TryParse(dateStr, out dateLong))
            {
                if (dateLong > 0)
                {
                    return new DateTime(1970, 01, 01).AddMilliseconds(dateLong);
                }
            }

            return null;
        }

        public string GetAddress(string addressStr)
        {
            return addressStr.TrimStart('+', '1');
        }

        public virtual bool HasFileToWrite()
        {
            return false;
        }

        public abstract void Initialize(XElement node, DirectoryInfo outputDir);

        public abstract string ComputeId();

        protected virtual byte[] ComputeHash()
        {
            return HashHelper.Instance.HashString(string.Format("{0}|{1}", MessageDate.Value.ToString("yyyy-MM-dd HH:mm:ss") ?? "<<null>>", Address ?? "<<null>>"));
        }
    }
}
