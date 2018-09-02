using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ParanoidDownloader.Core
{
    public class HttpDownloader
    {
        
        static HttpDownloader() => ServicePointManager.DefaultConnectionLimit = 512;

        private bool _stop = false;

        

        public string Url { get; set; }
        public string LocalFileName { get; set; }
        public ILogWriter Log { get; set; }
        public double Progress { get; set; }
        public int BufferSize { get; set; }
        public int ThreadCount { get; set; } = 8;

        public event EventHandler Completed;

        public readonly Dispatcher currentDispather;

        public List<DownloadThreadInfo> downloadThreadInfos;

        public HttpDownloader(string url, string localFileName, ILogWriter log)
        {
            Url = url;
            LocalFileName = localFileName;
            Log = log;
            currentDispather = Dispatcher.CurrentDispatcher;
        }

        public void StartDownload()
        {
            //测试连接
            var request = WebRequest.CreateHttp(Url);
            request.Method = "get";
            var repo = request.GetResponse();
            var length = repo.ContentLength;
            downloadThreadInfos = BuildDownloadThreadInfo(length, ThreadCount);
            downloadThreadInfos.ForEach(Console.WriteLine);
            Directory.CreateDirectory(Path.Combine(LocalFileName +DownloadingFileInfo.EXT));
            for (var i = 0; i < downloadThreadInfos.Count; i++) {
                downloadThreadInfos[i].Id = i;
                downloadThreadInfos[i].PartFile.FileName = Path.Combine(LocalFileName + DownloadingFileInfo.EXT, $"{i}");
                //创建并启动Task
                var i1 = i;
                downloadThreadInfos[i].Task = Task.Run(()=>DoDownload(downloadThreadInfos[i1]));
            }
        }


        private void DoDownload(DownloadThreadInfo info)
        {

            //Create a web connection
            var request = WebRequest.CreateHttp(Url);
            request.Method = "get";

            request.AddRange(info.PartFile.StartIndex, info.PartFile.EndIndex);
            Log.Info($"{info.Id} Send HTTP Get Request");
            var repo = request.GetResponse();
            var remote = repo.GetResponseStream();

            var local = File.Open(info.PartFile.FileName, FileMode.Create, FileAccess.ReadWrite);
            Log.Info($"{info.Id} Start download");
            var buffer = new byte[16 * 1024];
            while (true) {
                var bc = remote.Read(buffer, 0, buffer.Length);
                if (bc == 0) break;
                info.DownloadedByteCount += bc;
                local.Write(buffer, 0, bc);

                //停止
                if (_stop) {
                    break;
                }
            }
            local.Close();
            Log.Info($"{info.Id} exiting...");
            info.Completed = true;
            currentDispather.Invoke(() => NotifyComplete(info.Id));
        }


        async Task NotifyComplete(int id)
        {
            var goflag = true;
            foreach (var downloadThreadInfo in downloadThreadInfos) {
                if (downloadThreadInfo.Completed) continue;
                goflag = false;
                break;
            }

            if (goflag) {
                //所有下载完成
                //合并文件
                Log.Info("All thread exited. Start file combine.");
                var dst = File.Open(LocalFileName, FileMode.Create, FileAccess.ReadWrite);
                foreach (var downloadThreadInfo in downloadThreadInfos) {
                    var src = File.Open(downloadThreadInfo.PartFile.FileName, FileMode.Open, FileAccess.Read);
                    await src.CopyToAsync(dst);
                    src.Close();
                    File.Delete(downloadThreadInfo.PartFile.FileName);
                    Log.Info($"Write {downloadThreadInfo.PartFile.FileName} to {LocalFileName}");
                }

                dst.Close();
                Log.Info("All Completed!!!");
            }
            else {
            }
        }

        static List<DownloadThreadInfo> BuildDownloadThreadInfo(long length, int threadCount)
        {
            var pie = length / threadCount;
            var result = new List<DownloadThreadInfo>();
            for (var i = 0; i < threadCount - 1; i++) {
                result.Add(new DownloadThreadInfo() {
                    PartFile = new DownloadPartFile {
                        StartIndex = i * pie,
                        EndIndex = (i + 1) * pie - 1
                    }
                });
            }

            result.Add(new DownloadThreadInfo() {
                PartFile =  new DownloadPartFile {
                    StartIndex = (threadCount - 1) * pie,
                    EndIndex = length - 1,
                }
            });
            return result;
        }





        public async void Stop()
        {
            //标记停止信号
            _stop = true;

            //异步等待所有下载线程退出
            foreach (var downloadThreadInfo in downloadThreadInfos) {
                await downloadThreadInfo.Task;
            }

            //更新已下载的文件块信息

        }
    }

    public class DownloadThreadInfo
    {
        public int Id { get; set; }
        public DownloadPartFile PartFile { get; set; }
        public long DownloadedByteCount { get; set; }
        public bool Completed { get; set; }
        public Task Task { get; set; }

        public override string ToString() => $"Id:{Id}StartIndex:{PartFile.StartIndex}\nEndIndex:{PartFile.EndIndex}";
    }

    public class DownloadPartFile
    {
        public string FileName { get; set; }
        public long StartIndex { get; set; }
        public long EndIndex { get; set; }
        public bool Valid { get; set; }
    }
}
