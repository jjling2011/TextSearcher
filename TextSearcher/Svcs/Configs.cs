using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Svcs
{
    internal class Configs : IDisposable
    {
        const string file = @".\db\options.json";

        readonly Models.Options options;
        readonly object rwlock = new object();

        Configs()
        {
            options = LoadFromFile();
        }

        #region public
        public DateTime GetLastScan()
        {
            return options.lastScan;
        }

        public void SetLastScan(DateTime datetime)
        {
            lock (rwlock)
            {
                options.lastScan = datetime;
                SaveToFile();
            }
        }

        public List<int> GetListViewColWidths() => options.lvColWidths;

        public void SaveListViewColWidths(List<int> widths)
        {
            lock (rwlock)
            {
                options.lvColWidths = widths;
                SaveToFile();
            }
        }

        public bool IsFormMaximum()
        {
            return options.isFormMaximumSize;
        }

        public void SetFormMaximumSize(bool isMax)
        {
            if (options.isFormMaximumSize != isMax)
            {
                options.isFormMaximumSize = isMax;
                SaveToFile();
            }
        }

        public void SaveFolderInfos(List<Models.FolderInfo> folderInfos)
        {
            var fis = folderInfos.Where(fi => !string.IsNullOrEmpty(fi.folder)).Distinct().ToList();
            lock (rwlock)
            {
                options.folderInfos = fis;
                SaveToFile();
            }
        }

        public List<Models.FolderInfo> GetFolderInfos()
        {
            lock (rwlock)
            {
                return options.folderInfos;
            }
        }

        #endregion
        #region private
        void SaveToFile()
        {
            const string folder = "db";
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var text = JsonConvert.SerializeObject(options, Formatting.Indented);
                File.WriteAllText(file, text);
            }
            catch { }
        }

        Models.Options LoadFromFile()
        {
            try
            {
                var text = File.ReadAllText(file);
                var o = JsonConvert.DeserializeObject<Models.Options>(text);
                return o;
            }
            catch { }
            return new Models.Options();
        }
        #endregion

        #region singleton
        private static readonly Lazy<Configs> lazy = new Lazy<Configs>(() => new Configs());

        public static Configs Instance
        {
            get { return lazy.Value; }
        }
        #endregion

        #region IDisposable
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Lanucher()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
