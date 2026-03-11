using System;
using System.IO;
using Xunit;
using AngioPlayer.Services;

namespace AngioPlayer.TestProject
{
    public class ScanServiceTests
    {
        [Fact]
        public void LoadScans_ShouldPopulateScansCollection()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(Path.Combine(tempDir, "Scan1"));
            Directory.CreateDirectory(Path.Combine(tempDir, "Scan2"));

            var settings = new ScanSettings { ScansPath = tempDir };
            var service = new ScanService(settings);

            // Act
            service.LoadScans();

            // Assert
            Assert.Equal(2, service.Scans.Count);
            Assert.Contains("Scan1", service.Scans);
            Assert.Contains("Scan2", service.Scans);

            Directory.Delete(tempDir, true);
        }

        [Fact]
        public void GetScanPath_ShouldReturnFullPath()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(Path.Combine(tempDir, "Scan1"));

            var settings = new ScanSettings { ScansPath = tempDir };
            var service = new ScanService(settings);

            // Act
            var path = service.GetScanPath("Scan1");

            // Assert
            Assert.Equal(Path.Combine(tempDir, "Scan1"), path);

            Directory.Delete(tempDir, true);
        }

        [Fact]
        public void LoadScans_ShouldHandleNonExistingDirectoryGracefully()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var settings = new ScanSettings { ScansPath = tempDir };
            var service = new ScanService(settings);

            // Act
            Exception ex = Record.Exception(() => service.LoadScans());

            // Assert
            Assert.Null(ex);
            Assert.Empty(service.Scans);
        }

        [Fact]
        public void LoadScans_ShouldClearPreviousEntries()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(Path.Combine(tempDir, "Scan1"));

            var settings = new ScanSettings { ScansPath = tempDir };
            var service = new ScanService(settings);

            // Act
            service.LoadScans();
            Directory.CreateDirectory(Path.Combine(tempDir, "Scan2"));
            service.LoadScans();

            // Assert
            Assert.Equal(2, service.Scans.Count);
            Assert.Contains("Scan1", service.Scans);
            Assert.Contains("Scan2", service.Scans);
        }

        [Fact]
        public void LoadScans_ShouldHandleEmptyDirectory()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var settings = new ScanSettings { ScansPath = tempDir };
            var service = new ScanService(settings);

            // Act
            service.LoadScans();

            // Assert
            Assert.Empty(service.Scans);
        }
    }
}