using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IBackupFileSource fileSource = new LocalBackupFileSource(@"C:\Temp");

            DirectoryInfo OutputDir = new DirectoryInfo(Path.Combine("C:\\", "Temp", "SMSParseTest", "Output"));

            Dictionary<string, List<MessageBase>> Messages = new Dictionary<string, List<MessageBase>>();

            foreach (FileInfo sourceFile in fileSource.GetFiles())
            {
                using (FileStream FS = sourceFile.OpenRead())
                {
                    using (StreamReader SR = new StreamReader(FS, Encoding.UTF8))
                    {
                        using (XmlReader XR = XmlReader.Create(
                                                    FS,
                                                    new XmlReaderSettings()
                                                    {
                                                        IgnoreComments = true,
                                                        IgnoreProcessingInstructions = true,
                                                        IgnoreWhitespace = true,
                                                        CheckCharacters = false
                                                    }))
                        {
                            XR.MoveToContent();

                            while (XR.Read())
                            {
                                if (XR.NodeType == XmlNodeType.Element)
                                {
                                    if (XR.Name == "sms")
                                    {
                                        XElement SmsElement = XNode.ReadFrom(XR) as XElement;

                                        if (SmsElement != null)
                                        {
                                            SmsNode Result = new SmsNode();
                                            Result.Initialize(SmsElement, OutputDir);

                                            if (!string.IsNullOrEmpty(Result.Address))
                                            {
                                                if (!Messages.ContainsKey(Result.Address))
                                                    Messages.Add(Result.Address, new List<MessageBase>());

                                                Messages[Result.Address].Add(Result);
                                            }
                                        }
                                    }
                                    else if (XR.Name == "mms")
                                    {
                                        XElement MmsElement = XNode.ReadFrom(XR) as XElement;

                                        if (MmsElement != null)
                                        {
                                            MmsNode Result = new MmsNode();
                                            Result.Initialize(MmsElement, OutputDir);

                                            if (!string.IsNullOrEmpty(Result.Address))
                                            {
                                                if (!Messages.ContainsKey(Result.Address))
                                                    Messages.Add(Result.Address, new List<MessageBase>());

                                                Messages[Result.Address].Add(Result);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!OutputDir.Exists)
                OutputDir.Create();

            foreach (string key in Messages.Keys)
            {
                using (FileStream outputFS = new FileStream(Path.Combine(OutputDir.FullName, string.Format("SMSLog - {0}.txt", key)), FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter outputSW = new StreamWriter(outputFS))
                    {
                        Messages[key].Sort(Comparer<MessageBase>.Create((tmpNode1, tmpNode2) =>
                        {
                            if ((tmpNode1.MessageDate.HasValue) && (tmpNode2.MessageDate.HasValue))
                                return tmpNode1.MessageDate.Value.CompareTo(tmpNode2.MessageDate.Value);
                            else if (tmpNode1.MessageDate.HasValue)
                                return -1;
                            else if (tmpNode2.MessageDate.HasValue)
                                return 1;
                            else 
                                return 0;
   
                        }));
                        Messages[key].ForEach((node) => outputSW.WriteLine(node.ToString()));
                    }
                }

                // Write the images out to the file

            }
        }
    }
}
