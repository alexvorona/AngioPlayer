using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

public class ScanService
{
    private readonly string _path;
    private readonly DispatcherQueue _dispatcher;

    public ObservableCollection<string> Scans { get; } = new();

    public ScanService(string path, DispatcherQueue dispatcher)
    {
        _path = path;
        _dispatcher = dispatcher;

        if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

        LoadScans();

        var watcher = new FileSystemWatcher(_path)
        {
            NotifyFilter = NotifyFilters.DirectoryName,
            IncludeSubdirectories = false,
            EnableRaisingEvents = true
        };

        watcher.Created += (_, __) => LoadScans();
        watcher.Deleted += (_, __) => LoadScans();
        watcher.Renamed += (_, __) => LoadScans();
    }

    private void LoadScans()
    {
        Task.Run(() =>
        {
            var folders = Directory.GetDirectories(_path)
                                   .Select(Path.GetFileName)
                                   .OrderBy(name => name)
                                   .ToList();

            _dispatcher.TryEnqueue(() =>
            {
                Scans.Clear();
                foreach (var folder in folders)
                    Scans.Add(folder);
            });
        });
    }
}