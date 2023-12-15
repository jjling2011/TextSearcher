using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSearcher.Svcs
{
    internal class DbMgr : IDisposable
    {
        public event Action<string> onMessage;
        public event Action<string> onStatus;

        DbMgr() { }

        #region public
        public void Clear()
        {
            Msg("Clearing...");
            Exec(db =>
            {
                var records = db.TextFiles.ToList();
                db.TextFiles.RemoveRange(records);
                db.SaveChanges();
            });
            Msg("Ready!");
        }

        public void Compact()
        {
            Msg("Deleting...");
            Exec(db =>
            {
                var records = db.TextFiles.Where(fi => fi.deleted).ToList();
                foreach (var record in records)
                {
                    Msg($"Delete file: {record.path}");
                    db.TextFiles.Remove(record);
                }
                db.SaveChanges();
            });
            Msg("Ready!");
        }

        public List<Models.TextFileInfo> Search(List<string> keywords)
        {
            Msg("Searching...");
            var records =
                keywords == null || keywords.Count < 1
                    ? GetTop100Records()
                    : GetMatchedRecords(keywords);
            Msg($"{records.Count} records");
            return records;
        }

        public string GetContent(string path)
        {
            var content = "";
            Exec(db =>
            {
                var record = db.TextFiles.FirstOrDefault(fi => fi.path == path);
                if (record != null)
                {
                    content = record.content;
                }
            });
            return content;
        }

        public void Scan()
        {
            Msg("Marking...");
            MarkNotExistFiles();

            Msg("Scanning...");
            var fis = Configs.Instance.GetFolderInfos().Where(fi => fi.isScan).ToList();

            var c = 0;
            foreach (var fi in fis)
            {
                if (isClosing)
                {
                    return;
                }
                Msg($"Scan: {fi.folder}");
                Exec(db =>
                {
                    try
                    {
                        c += ScanOneFolder(db, fi);
                    }
                    catch { }
                    db.SaveChanges();
                });
            }
            Configs.Instance.UpdateLastScanTimestamp();
            Msg($"Ready! {c} new records.");
        }

        #endregion

        #region private
        int ScanOneFolder(Models.LocalDbContext db, Models.FolderInfo fi)
        {
            const int SaveChangesSize = 25 * 1024 * 1024;

            var exts = ParseExtensions(fi.exts);
            var c = 0;
            var size = 0;

            foreach (
                var file in Directory.EnumerateFiles(fi.folder, "*.*", SearchOption.AllDirectories)
            )
            {
                var ext = Path.GetExtension(file);
                if (!ValidateExtension(exts, ext))
                {
                    continue;
                }

                var modify = File.GetLastWriteTime(file);
                if (IsDuplicated(file, modify))
                {
                    Msg($"Duplicated: {file}");
                    continue;
                }

                Msg($"Add: {file}");
                c++;
                size += AddToDb(db, file, modify, ext);
                if (size > SaveChangesSize)
                {
                    size = 0;
                    db.SaveChanges();
                }
            }
            return c;
        }

        List<string> ParseExtensions(string exts)
        {
            return exts?.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.Select(e => $".{e}")
                ?.ToList();
        }

        void MarkNotExistFiles()
        {
            Exec(db =>
            {
                var records = db.TextFiles.Where(fi => !fi.deleted);
                foreach (var record in records)
                {
                    var file = record.path;
                    if (File.Exists(file))
                    {
                        continue;
                    }
                    Msg($"Remove file: {file}");
                    record.deleted = true;
                }
                db.SaveChanges();
            });
        }

        private List<Models.TextFileInfo> GetMatchedRecords(List<string> kws)
        {
            var excludedFolders = Configs.Instance
                .GetFolderInfos()
                .Where(f => !f.isSearch)
                .Select(f => f.folder)
                .ToList();

            var r = new List<Models.TextFileInfo>();
            Exec(db =>
            {
                var q = db.TextFiles.Where(fi => !fi.deleted);
                for (int i = 0; i < excludedFolders.Count; i++)
                {
                    var f = excludedFolders[i];
                    q = q.Where(fi => !fi.path.StartsWith(f));
                }

                for (int i = 0; i < kws.Count; i++)
                {
                    var kw = kws[i];
                    q = q.Where(fi => fi.content.Contains(kw));
                }
                r = q.ToList();
            });
            return r;
        }

        private List<Models.TextFileInfo> GetTop100Records()
        {
            var r = new List<Models.TextFileInfo>();
            Exec(db =>
            {
                r = db.TextFiles
                    .Where(fi => !fi.deleted)
                    .OrderByDescending(fi => fi.modify)
                    .Take(100)
                    .ToList();
            });
            return r;
        }

        bool IsDuplicated(string path, DateTime modify)
        {
            Models.TextFileInfo record = null;
            Exec(db =>
            {
                record = db.TextFiles
                    .Where(fi => fi.path == path)
                    .Where(fi => fi.modify == modify)
                    .FirstOrDefault();

                if (record != null && record.deleted)
                {
                    record.deleted = false;
                    db.SaveChanges();
                }
            });
            return record != null;
        }

        int AddToDb(Models.LocalDbContext db, string file, DateTime modify, string ext)
        {
            var fi = ReadFile(file, modify, ext);
            if (fi == null)
            {
                Msg($"Error: {file}");
                return 0;
            }

            var record = db.TextFiles.FirstOrDefault(info => info.path == file);
            if (record == null)
            {
                db.TextFiles.Add(fi);
            }
            else
            {
                record.Copy(fi);
            }

            return fi.content.Length;
        }

        Models.TextFileInfo ReadFile(string file, DateTime modify, string ext)
        {
            try
            {
                var content =
                    File.ReadAllText(file)?.Replace("\r", "")?.Replace("\n", " ")?.ToLower() ?? "";

                return new Models.TextFileInfo()
                {
                    modify = modify,
                    content = content,
                    ext = ext,
                    file = Path.GetFileName(file),
                    path = file,
                    deleted = false,
                };
            }
            catch { }
            return null;
        }

        bool ValidateExtension(List<string> exts, string ext)
        {
            if (exts != null && !string.IsNullOrEmpty(ext) && exts.Count > 0)
            {
                return exts.Contains(ext);
            }
            return false;
        }

        string newMsg;

        void Msg(string text)
        {
            newMsg = text;
            onMessage?.Invoke(text);
            UpdateStatus();
        }

        string curMsg;
        bool waiting = false;
        long stamp;

        void UpdateStatus()
        {
            const int UPDATE_INTERVAL = 1;

            if (newMsg == curMsg || waiting)
            {
                return;
            }

            void update()
            {
                waiting = false;
                curMsg = newMsg;
                stamp = DateTime.Now.Ticks;
                onStatus?.Invoke(curMsg);
            }

            if (DateTime.Now.Ticks - stamp > UPDATE_INTERVAL * TimeSpan.TicksPerSecond)
            {
                update();
            }
            else
            {
                waiting = true;
                Task.Delay(TimeSpan.FromSeconds(UPDATE_INTERVAL)).ContinueWith(_ => update());
            }
        }

        readonly object dbRwlock = new object();
        bool isClosing = false;

        void Exec(Action<Models.LocalDbContext> job)
        {
            lock (dbRwlock)
            {
                if (isClosing)
                {
                    return;
                }
                using (var db = new Models.LocalDbContext())
                {
                    job(db);
                }
            }
        }
        #endregion

        #region singleton
        private static readonly Lazy<DbMgr> lazy = new Lazy<DbMgr>(() => new DbMgr());

        public static DbMgr Instance
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
                    isClosing = true;
                    lock (dbRwlock)
                    {
                        Console.WriteLine("Waiting to exit...");
                    }
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
