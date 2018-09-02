using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParanoidDownloader.Core
{
    [Serializable]
    class DownloadingFileInfo
    {
        public const string EXT = ".paranoid_downloading";

        public TimeSpan TimeCost { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public long FileSize { get; set; }
        
        public DownloadPartFile PartFiles { get; set; }
    }
}
