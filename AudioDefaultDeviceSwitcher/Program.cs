using System;
using System.IO;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioDefaultDeviceSwitcher {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run();
        }
        private const string IniFileName = "Audio.ini";
        private const string DevicesIniKey = "Devices";
        private const string AlsoSetCommunicationsIniKey = "AlsoSetCommunications";
        private const string FixSkypeIniKey = "FixSkype";
        private readonly string _iniPath = AppDomain.CurrentDomain.BaseDirectory + IniFileName;
        private readonly string _logPath = AppDomain.CurrentDomain.BaseDirectory + "Audio.log";

        public void Run() {
            Initialize();
            var ini = new EasyIni(_iniPath);
            var devicesStr = ini.Val(DevicesIniKey);
            var devices = devicesStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (devices.Length <= 1) {
                CreateAndWriteToFile(_logPath, "Please enter more than one device-name.");
                Environment.Exit(0);
            }
            var activePlayback = Controller.GetPlaybackDevices(DeviceState.Active);
            var relevantDevicesArr = activePlayback.Where(o => devices.Contains(o.Name)).ToArray();
            if (relevantDevicesArr.Length <= 1) {
                CreateAndWriteToFile(_logPath, "Could not find more than one device to togglet between. Did you type the names correct?");
                Environment.Exit(0);
            }
            var currentDefaultDevice = relevantDevicesArr.FirstOrDefault(o => o.IsDefaultDevice);
            var currentIndex = Array.IndexOf(relevantDevicesArr, currentDefaultDevice);
            CoreAudioDevice nextDevice = currentIndex == relevantDevicesArr.Length - 1
                ? relevantDevicesArr.First()
                : relevantDevicesArr[currentIndex + 1];

            nextDevice.SetAsDefault();
            var alsoCommunications = ini.Val(AlsoSetCommunicationsIniKey, true);
            if (alsoCommunications)
                nextDevice.SetAsDefaultCommunications();

            var fixSkype = ini.Val(FixSkypeIniKey, false);
            if (fixSkype) {
                try {
                    new SkypeFixer(nextDevice).SetAudioOut();
                } catch (Exception ex) {
                    CreateAndWriteToFile(_logPath, "Fixing skype failed. Exception: " + ex);
                    Environment.Exit(0);
                }
            }
        }

        private void Initialize() {
            if (!File.Exists(_iniPath)) {
                var defaultIni = DevicesIniKey + "=A,B,C" + Environment.NewLine
                    + AlsoSetCommunicationsIniKey + "=true" + Environment.NewLine
                    + FixSkypeIniKey + "=false";
                CreateAndWriteToFile(_iniPath, defaultIni);
                CreateAndWriteToFile(_logPath, string.Format("A {0} has been created for you, fill it with the playback devices you want to toggle between.\r\nPath to the .ini: {1}", IniFileName, _iniPath));
                Environment.Exit(0);
            }
        }

        private void CreateAndWriteToFile(string filePath, string text) {
            using (var stream = File.CreateText(filePath)) {
                stream.WriteLine(text);
            }
        }

        public CoreAudioController Controller = new CoreAudioController();
    }
}
