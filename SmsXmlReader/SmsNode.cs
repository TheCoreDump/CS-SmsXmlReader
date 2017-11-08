using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class SmsNode : MessageBase
    {
        public SmsNode() 
            : base()
        {
        }

        public string Body { get; set; }

        public SmsMessageType MessageType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : SMS message (ID: {1}): {2} ({3}) : {4}", 
                MessageDate.HasValue ? MessageDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "<< Unknown >>", 
                Id,
                MessageType == SmsMessageType.Incoming ? string.Format("{0} -> Me", ContactName) : string.Format("Me -> {0}", ContactName), 
                Address, 
                Body);
        }


        public override void Initialize(XElement node, DirectoryInfo outputDir)
        {
            foreach (XAttribute tmpAttr in node.Attributes())
            {
                if (tmpAttr.Name == "address")
                    Address = GetAddress(tmpAttr.Value);

                if (tmpAttr.Name == "body")
                    Body = tmpAttr.Value;

                if (tmpAttr.Name == "contact_name")
                    ContactName = tmpAttr.Value;

                if (tmpAttr.Name == "date")
                    MessageDate = GetDate(tmpAttr.Value);

                if (tmpAttr.Name == "type")
                {
                    if (tmpAttr.Value == "1")
                        MessageType = SmsMessageType.Incoming;
                    else
                        MessageType = SmsMessageType.Outgoing;
                }
            }
        }


        private string _id;

        public override string ComputeId()
        {
            return Convert.ToBase64String(HashHelper.CombineHashes(ComputeHash(), HashHelper.Instance.HashString(Body)));
        }
    }
}
