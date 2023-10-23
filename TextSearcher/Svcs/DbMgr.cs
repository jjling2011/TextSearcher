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

        readonly Models.LocalDbContext db = new Models.LocalDbContext();

        DbMgr() { }

        #region public
        public void Clear()
        {
            Msg("Clearing ...");
            lock (db)
            {
                var records = db.TextFiles.ToList();
                db.TextFiles.RemoveRange(records);
                db.SaveChanges();
            }
            Msg("Ready!");
        }

        public void Compact()
        {
            Msg("Deleting ...");
            lock (db)
            {
                var records = db.TextFiles.Where(fi => fi.deleted).ToList();
                foreach (var record in records)
                {
                    Msg($"Delete file: {record.path}");
                    db.TextFiles.Remove(record);
                }
                db.SaveChanges();
            }
            Msg("Ready!");
        }

        public List<Models.TextFileInfo> Search(string keywords)
        {
            var kws = keywords
                ?.ToLower()
                ?.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                ?.ToList();
            Msg("Searching...");
            var records =
                kws == null || kws.Count < 1 ? GetTop100Records() : GetMatchedRecords(kws);
            Msg($"{records.Count} records");
            return records;
        }

        public void Scan()
        {
            Msg("Marking ...");
            MarkNotExistFiles();
            Msg("Scanning ...");
            var fis = Configs.Instance.GetFolderInfos();
            const int SaveChangesSize = 25 * 1024 * 1024;
            var size = 0;
            foreach (var fi in fis)
            {
                if (!fi.isScan)
                {
                    continue;
                }

                var folder = fi.folder;
                var exts = fi.exts
                    ?.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    ?.Select(e => $".{e}")
                    ?.ToList();

                Msg($"Scan folder: {folder}");
                foreach (
                    var file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
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
                    size += AddToDb(file, modify, ext);
                    if (size > SaveChangesSize)
                    {
                        size = 0;
                        SaveChanges();
                    }
                }
            }
            SaveChanges();
            Configs.Instance.SetLastScan(DateTime.Now);
            Msg("Ready!");
        }
        #endregion

        #region private
        void SaveChanges()
        {
            lock (db)
            {
                db.SaveChanges();
            }
        }

        void MarkNotExistFiles()
        {
            lock (db)
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
            }
        }

        private List<Models.TextFileInfo> GetMatchedRecords(List<string> kws)
        {
            IQueryable<Models.TextFileInfo> q;

            var folders = Configs.Instance
                .GetFolderInfos()
                .Where(f => !f.isSearch)
                .Select(f => f.folder)
                .ToList();

            lock (db)
            {
                q = db.TextFiles.Where(fi => !fi.deleted);
                for (int i = 0; i < folders.Count; i++)
                {
                    var f = folders[i];
                    q = q.Where(fi => !fi.path.StartsWith(f));
                }

                for (int i = 0; i < kws.Count; i++)
                {
                    var kw = kws[i];
                    q = q.Where(fi => fi.content.Contains(kw));
                }
                return q.ToList();
            }
        }

        private List<Models.TextFileInfo> GetTop100Records()
        {
            lock (db)
            {
                return db.TextFiles.Where(fi => !fi.deleted).Take(100).ToList();
            }
        }

        bool IsDuplicated(string path, DateTime modify)
        {
            lock (db)
            {
                var record = db.TextFiles
                    .Where(fi => fi.path == path)
                    .Where(fi => fi.modify == modify)
                    .FirstOrDefault();

                if (record != null && record.deleted)
                {
                    record.deleted = false;
                    db.SaveChanges();
                }
                return record != null;
            }
        }

        int AddToDb(string file, DateTime modify, string ext)
        {
            var fi = new Models.TextFileInfo()
            {
                modify = modify,
                content =
                    File.ReadAllText(file)?.Replace("\r", "")?.Replace("\n", " ")?.ToLower() ?? "",
                ext = ext,
                file = Path.GetFileName(file),
                path = file,
                deleted = false,
            };

            lock (db)
            {
                var record = db.TextFiles.FirstOrDefault(info => info.path == file);
                if (record == null)
                {
                    db.TextFiles.Add(fi);
                }
                else
                {
                    record.ext = fi.ext;
                    record.content = fi.content;
                    record.modify = fi.modify;
                    record.file = fi.file;
                    record.path = fi.path;
                    record.deleted = fi.deleted;
                }
            }
            return fi.content.Length;
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

            if (DateTime.Now.Ticks - stamp > 2 * TimeSpan.TicksPerSecond)
            {
                update();
            }
            else
            {
                waiting = true;
                Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith(_ => update());
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
                    db?.Dispose();
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
