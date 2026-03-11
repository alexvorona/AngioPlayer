using System;
using System.Collections.ObjectModel;
using System.IO;
using AngioPlayer.Services;
using AngioPlayer.ViewModels;
using Microsoft.UI.Xaml.Media;
using Moq;
using Xunit;

namespace AngioPlayer.TestProject
{
    public class PlayerControlViewModelTests
    {
        private readonly Mock<IScanService> _mockScanService;
        private readonly Mock<INotificationService> _mockNotifications;
        private readonly Mock<ITimerService> _mockTimer;

        public PlayerControlViewModelTests()
        {
            _mockScanService = new Mock<IScanService>();
            _mockNotifications = new Mock<INotificationService>();
            _mockTimer = new Mock<ITimerService>();
            _mockTimer.SetupAllProperties();
        }

        private PlayerControlViewModel CreateViewModel()
        {
            return new PlayerControlViewModel(
                _mockScanService.Object,
                _mockNotifications.Object,
                _mockTimer.Object);
        }

        [Fact]
        public void Scans_ShouldReflectScanService()
        {
            // Arrange
            var scans = new ObservableCollection<string> { "Scan1", "Scan2" };
            _mockScanService.Setup(s => s.Scans).Returns(scans);
            var vm = CreateViewModel();

            // Act
            var vmScans = vm.Scans;

            // Assert
            Assert.Equal(2, vmScans.Count);
            Assert.Contains("Scan1", vmScans);
            Assert.Contains("Scan2", vmScans);
        }

        [Fact]
        public void SelectedSpeed_ShouldUpdateTimerInterval()
        {
            // Arrange
            var vm = CreateViewModel();

            // Act
            vm.SelectedSpeed = "Fast";

            // Assert
            Assert.Equal("Fast", vm.SelectedSpeed);
            Assert.Equal(TimeSpan.FromMilliseconds(100), _mockTimer.Object.Interval);
        }

        [Fact]
        public void Next_ShouldIncrementCurrentFrame()
        {
            // Arrange
            var vm = CreateViewModel();
            
            var planeABitmaps = new List<ImageSource> { null, null, null, null, null };
            var planeBBitmaps = new List<ImageSource> { null, null, null, null, null };

            typeof(PlayerControlViewModel)
                .GetField("_planeABitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeABitmaps);

            typeof(PlayerControlViewModel)
                .GetField("_planeBBitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeBBitmaps);

            typeof(PlayerControlViewModel)
                .GetField("_imageCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, 5);

            vm.SliderMax = 4;
            vm.CurrentFrame = 2;

            // Act
            vm.NextCommand.Execute(null);

            // Assert
            Assert.Equal(3, vm.CurrentFrame);
        }

        [Fact]
        public void Prev_ShouldDecrementCurrentFrame()
        {
            // Arrange
            var vm = CreateViewModel();

            var planeABitmaps = new List<ImageSource> { null, null, null, null, null };
            var planeBBitmaps = new List<ImageSource> { null, null, null, null, null };

            typeof(PlayerControlViewModel)
                .GetField("_planeABitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeABitmaps);

            typeof(PlayerControlViewModel)
                .GetField("_planeBBitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeBBitmaps);

            typeof(PlayerControlViewModel)
                .GetField("_imageCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, 5);

            vm.CurrentFrame = 2;

            // Act
            vm.PrevCommand.Execute(null);

            // Assert
            Assert.Equal(1, vm.CurrentFrame);
        }

        [Fact]
        public void KeyImage_ShouldSetFrameToMiddle()
        {
            // Arrange
            var vm = CreateViewModel();

            var planeABitmaps = new List<ImageSource> { null, null, null, null, null, null, null, null, null, null };
            var planeBBitmaps = new List<ImageSource> { null, null, null, null, null, null, null, null, null, null };

            typeof(PlayerControlViewModel)
                .GetField("_planeABitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeABitmaps);

            typeof(PlayerControlViewModel)
                .GetField("_planeBBitmaps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, planeBBitmaps);
            typeof(PlayerControlViewModel)
                .GetField("_imageCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(vm, 10);
            vm.CurrentFrame = 0;

            // Act
            vm.KeyImageCommand.Execute(null);

            // Assert
            Assert.Equal(5, vm.CurrentFrame);
        }

        [Fact]
        public void Load_ShouldShowErrorIfDirectoriesMissing()
        {
            // Arrange
            _mockScanService.Setup(s => s.GetScanPath(It.IsAny<string>())).Returns(Path.GetTempPath());
            var vm = CreateViewModel();
            vm.SelectedScan = "Scan1";

            // Act
            vm.LoadCommand.Execute(null);

            // Assert
            _mockNotifications.Verify(n => n.ShowError(It.IsAny<string>()), Times.Once);
        }
    }
}