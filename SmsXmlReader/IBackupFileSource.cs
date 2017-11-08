using System.Collections.Generic;
using System.IO;

namespace SmsXmlReader
{
    public interface IBackupFileSource 
    {
        IEnumerable<FileInfo> GetFiles();
    }
}
