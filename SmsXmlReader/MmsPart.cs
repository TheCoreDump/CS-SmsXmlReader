using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class MmsPart
    {
        protected MmsPart()
        {
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public static MmsPart Create(MmsNode parentNode, XElement partNode, DirectoryInfo outputDir, int partIndex)
        {
            XAttribute typeAttr = partNode.Attributes("ct").FirstOrDefault();

            if (typeAttr == null)
                throw new ApplicationException("Missing part type attribute");


            if ((typeAttr.Value == "image/jpeg") || (typeAttr.Value == "image/png"))
            {
                MmsPartImage ImgResult = new MmsPartImage();
                ImgResult.Initialize(parentNode, partNode, outputDir, partIndex);

                return ImgResult;
            }

            if (typeAttr.Value == "text/plain")
            {
                MmsPartText TxtResult = new MmsPartText();
                TxtResult.Initialize(partNode);

                return TxtResult;
            }

            Console.WriteLine("Unknown part type: {0}", typeAttr.Value);

            MmsPart BaseResult = new MmsPart();
            BaseResult.Initialize(partNode);

            return BaseResult;
        }


        public virtual bool HasFileToWrite()
        {
            return false;
        }

        public virtual void WriteToFile()
        {

        }

        public virtual byte[] GetHash()
        {
            return HashHelper.Instance.HashString(string.Format("{0}|{1}", Name, Type));
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Type);
        }

        public virtual void Initialize(XElement node)
        {
            foreach (XAttribute tmpAttr in node.Attributes())
            {
                if (tmpAttr.Name == "ct")
                    Type = tmpAttr.Value;

                if (tmpAttr.Name == "name")
                    Name = tmpAttr.Value;
            }
        }
    }
}
