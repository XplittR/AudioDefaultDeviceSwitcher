using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioDefaultDeviceSwitcher {
    public class Program {
        public static async Task Main(string[] args) {
            await new Program().Run();
        }
        private const string IniFileName = "Audio.ini";
        private const string DevicesIniKey = "Devices";
        private const string AlsoSetCommunicationsIniKey = "AlsoSetCommunications";
        private const string FixSkypeIniKey = "FixSkype";
        private readonly string _iniPath = AppDomain.CurrentDomain.BaseDirectory + IniFileName;
        private readonly string _logPath = AppDomain.CurrentDomain.BaseDirectory + "Audio.log";

        public async Task Run() {
            Initialize();
            ReadIni();

            var activePlayback = await Controller.GetPlaybackDevicesAsync(DeviceState.Active);
            var relevantDevicesArr = activePlayback.Where(o => _devices.Contains(o.Name)).ToArray();
            if (relevantDevicesArr.Length <= 1) {
                CreateAndWriteToFile(_logPath, "Could not find more than one device to togglet between. Did you type the names correct?");
                Environment.Exit(0);
            }
            var currentDefaultDevice = relevantDevicesArr.FirstOrDefault(o => o.IsDefaultDevice);
            var currentIndex = Array.IndexOf(relevantDevicesArr, currentDefaultDevice);
            _nextDevice = currentIndex == relevantDevicesArr.Length - 1
                ? relevantDevicesArr.First()
                : relevantDevicesArr[currentIndex + 1];

            var defDeviceTask = SetDefaultDevice();
            var defComDeviceTask = SetDefaultCommunicationsDevice();
            var skypeFixTask = FixSkype();

            Task.WaitAll(defDeviceTask, defComDeviceTask, skypeFixTask);
        }

        private Task FixSkype() {
            var fixSkype = _ini.Val(FixSkypeIniKey, false);
            if (!fixSkype)
                return Task.CompletedTask;
            try {
                new SkypeFixer(_nextDevice).SetAudioOut();
            } catch (Exception ex) {
                CreateAndWriteToFile(_logPath, "Fixing skype failed. Exception: " + ex);
                Environment.Exit(0);
            }
            return Task.CompletedTask;
        }

        private async Task SetDefaultCommunicationsDevice() {
            var alsoCommunications = _ini.Val(AlsoSetCommunicationsIniKey, true);
            if (alsoCommunications) {
                await _nextDevice.SetAsDefaultCommunicationsAsync();
            }
        }

        private async Task SetDefaultDevice() {
            await _nextDevice.SetAsDefaultAsync();
        }

        private void ReadIni() {
            _ini = new EasyIni(_iniPath);
            var devicesStr = _ini.Val(DevicesIniKey);
            _devices = devicesStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (_devices.Length <= 1) {
                CreateAndWriteToFile(_logPath, "Please enter more than one device-name.");
                Environment.Exit(0);
            }
        }

        private void Initialize() {
            if (!File.Exists(_iniPath)) {
                var defaultIni = DevicesIniKey + "=A,B,C" + Environment.NewLine
                    + AlsoSetCommunicationsIniKey + "=true" + Environment.NewLine
                    + FixSkypeIniKey + "=false";
                CreateAndWriteToFile(_iniPath, defaultIni);
                CreateAndWriteToFile(_logPath, $"A {IniFileName} has been created for you, fill it with the playback devices you want to toggle between.\r\nPath to the .ini: {_iniPath}");
                Environment.Exit(0);
            }
        }

        private void CreateAndWriteToFile(string filePath, string text) {
            using (var stream = File.CreateText(filePath)) {
                stream.WriteLine(text);
            }
        }

        public CoreAudioController Controller = new CoreAudioController();
        private EasyIni _ini;
        private string[] _devices;
        private CoreAudioDevice _nextDevice;
    }
}
