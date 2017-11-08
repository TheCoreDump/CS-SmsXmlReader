using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class MmsNode : MessageBase
    {
        private Func<string, byte[]> _hashBehavior;

        public MmsNode()
            : base()
        {
        }


        public List<MmsPart> Parts { get; set; }

        public List<string> Addresses { get; set; }

        public string Name { get; set; }


        public override string ToString()
        {
            StringBuilder Buffer = new StringBuilder();

            Buffer.AppendFormat("{0} : MMS message (ID: {1}) {2} ({3})",
                MessageDate.HasValue ? MessageDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "<< Unknown >>",
                Id,
                ContactName,
                Address);

            Buffer.AppendLine();

            Addresses.ForEach((addr) =>
            {
                Buffer.AppendFormat(" Address: {0}", addr);
                Buffer.AppendLine();
            });

            Parts.ForEach((part) =>
            {
                Buffer.AppendFormat(" Part: {0}", part.ToString());
                Buffer.AppendLine();
            });
            
            return Buffer.ToString();
        }


        public override void Initialize(XElement node, DirectoryInfo outputDir)
        {
            Addresses = new List<string>();
            Parts = new List<MmsPart>();

            foreach (XAttribute tmpAttr in node.Attributes())
            {
                if (tmpAttr.Name == "date")
                    MessageDate = GetDate(tmpAttr.Value);

                if (tmpAttr.Name == "address")
                    Address = GetAddress(tmpAttr.Value);

                if (tmpAttr.Name == "contact_name")
                    ContactName = tmpAttr.Value;

                if (tmpAttr.Name == "name")
                    Name = tmpAttr.Value;

            }


            foreach (XElement tmpElement in node.Elements())
            {
                if (tmpElement.Name == "addrs")
                {
                    if (tmpElement != null)
                    {
                        foreach (XElement tmpAddress in tmpElement.Elements())
                        {
                            if (tmpAddress.Name == "addr")
                            {
                                foreach (XAttribute tmpAddrAttr in tmpAddress.Attributes())
                                {
                                    if (tmpAddrAttr.Name == "address")
                                        Addresses.Add(GetAddress(tmpAddrAttr.Value));
                                }
                            }
                        }
                    }
                }
                else if (tmpElement.Name == "parts")
                {
                    if (tmpElement != null)
                    {
                        int partIndex = 0;

                        foreach (XElement tmpPart in tmpElement.Elements())
                        {
                            if (tmpPart.Name == "part")
                                Parts.Add(MmsPart.Create(this, tmpPart, outputDir, partIndex));
                        }
                    }
                }
            }
        }

        public override string ComputeId()
        {
            byte[] Result = ComputeHash();

            if (null != Addresses)
                Addresses.ForEach((a) => Result = HashHelper.CombineHashes(HashHelper.Instance.HashString(a), Result));

            if (null != Parts)
                Parts.ForEach((p) => Result = HashHelper.CombineHashes(p.GetHash(), Result));

            return Convert.ToBase64String(Result);
        }
    }
}
