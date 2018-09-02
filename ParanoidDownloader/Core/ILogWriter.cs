using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParanoidDownloader.Core
{
    public interface ILogWriter
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);

    }
}
