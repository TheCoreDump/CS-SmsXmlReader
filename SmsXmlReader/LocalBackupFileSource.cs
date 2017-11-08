using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmsXmlReader
{
    public class LocalBackupFileSource : IBackupFileSource
    {
        Regex smsBackupFileNameRegex = new Regex(@"sms-[0-9]{14}\.xml", RegexOptions.Compiled);

        public LocalBackupFileSource(string directory)
        {
            Directory = new DirectoryInfo(directory);
        }

        public DirectoryInfo Directory { get; set; }


        public IEnumerable<FileInfo> GetFiles()
        {
            return Directory.GetFiles("*.xml").ToList().Where(fi => smsBackupFileNameRegex.IsMatch(fi.Name));
        }
    }
}
