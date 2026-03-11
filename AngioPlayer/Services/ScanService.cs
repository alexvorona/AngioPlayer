using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace AngioPlayer.Services;

public interface IScanService
{
    ObservableCollection<string> Scans { get; }
    void LoadScans();
    string GetScanPath(string scanName);
}
public class ScanService : IScanService
{
    private readonly string _scansPath;
    private readonly DispatcherQueue _dispatcher;

    public ObservableCollection<string> Scans { get; } = new();

    public ScanService(ScanSettings settings, DispatcherQueue dispatcher)
    {
        _scansPath = settings.ScansPath;
        _dispatcher = dispatcher;

        LoadScans();
    }

    public void LoadScans()
    {
        if (!Directory.Exists(_scansPath))
            return;

        var folders = Directory.GetDirectories(_scansPath)
                               .Select(Path.GetFileName)
                               .OrderBy(f => f);

        foreach (var folder in folders)
        {
            Scans.Add(folder);
        }
    }

    public string GetScanPath(string scanName)
    {
        var fullPath = Path.Combine(_scansPath, scanName);
        return fullPath;
    }
}

public class ScanSettings
{
    public string ScansPath { get; set; } = string.Empty;
}