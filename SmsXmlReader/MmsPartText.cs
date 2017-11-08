using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SmsXmlReader
{
    public class MmsPartText : MmsPart
    {
        public MmsPartText()
            : base()
        {
        }


        public string Text { get; set; }


        public override string ToString()
        {
            return string.Format("{0} ({1}) - Text: {2}", Name, Type, Text);
        }

        public override void Initialize(XElement node)
        {
            base.Initialize(node);

            foreach (XAttribute tmpAttr in node.Attributes())
            {
                if (tmpAttr.Name == "text")
                    Text = tmpAttr.Value;
            }
        }

        public override byte[] GetHash()
        {
            return HashHelper.CombineHashes(HashHelper.Instance.HashString(Text), base.GetHash());
        }
    }
}
