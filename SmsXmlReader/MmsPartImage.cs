using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class MmsPartImage : MmsPart
    {
        public MmsPartImage()
            : base()
        {
        }

        public string FileName { get; set; }

        public override bool HasFileToWrite()
        {
            return true;
        }

        public void Initialize(MmsNode parentNode, XElement node, DirectoryInfo outputDir, int imageIndex)
        {
            base.Initialize(node);

            byte[] byteBuffer = new byte[4096];

            if (parentNode.MessageDate.HasValue)
            {
                DirectoryInfo ImgDirectory = new DirectoryInfo(Path.Combine(outputDir.FullName, parentNode.MessageDate.Value.ToString("yyyy-MM-dd")));

                if (!ImgDirectory.Exists)
                    ImgDirectory.Create();

                FileName = Path.Combine(ImgDirectory.FullName, string.Format("{0}-{1}.{2}", parentNode.MessageDate.Value.ToString("yyyyMMdd-HHmmss"), imageIndex, GetExtension()));
            }
            else
            {
                DirectoryInfo ImgDirectory = new DirectoryInfo(Path.Combine(outputDir.FullName, "Unknown"));

                if (!ImgDirectory.Exists)
                    ImgDirectory.Create();

                FileName = Path.Combine(ImgDirectory.FullName, string.Format("{0}.jpg", Guid.NewGuid().ToString("d")));
            }

            foreach (XAttribute tmpAttr in node.Attributes())
            {
                if (tmpAttr.Name == "data")
                {
                    using (MemoryStream SourceMS = new MemoryStream(Encoding.UTF8.GetBytes(tmpAttr.Value)))
                    {
                        using (CryptoStream CS = new CryptoStream(SourceMS, new FromBase64Transform(), CryptoStreamMode.Read))
                        {
                            using (FileStream imageFS = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                            {
                                int bytesRead = CS.Read(byteBuffer, 0, byteBuffer.Length);

                                while (bytesRead > 0)
                                {
                                    imageFS.Write(byteBuffer, 0, bytesRead);
                                    bytesRead = CS.Read(byteBuffer, 0, byteBuffer.Length);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetExtension()
        {
            switch (Type)
            {
                case "image/jpeg":
                    return "jpg";
                case "image/png":
                    return "png";
            }

            return "unknown";
        }

        public override byte[] GetHash()
        {
            FileInfo fi = new FileInfo(FileName);

            return HashHelper.Instance.HashStream(fi.OpenRead());
        }
    }
}
