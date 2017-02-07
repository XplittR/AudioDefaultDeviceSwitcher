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
        private readonly string _iniPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + IniFileName;

        public void Run() {
            Initialize();
            var ini = new EasyIni(_iniPath);
            var devicesStr = ini.Val(DevicesIniKey);
            var devices = devicesStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (devices.Length <= 1) {
                Console.WriteLine("Please enter more than one device-name.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            var activePlayback = Controller.GetPlaybackDevices(DeviceState.Active);
            var relevantDevicesArr = activePlayback.Where(o => devices.Contains(o.Name)).ToArray();
            if (relevantDevicesArr.Length <= 1) {
                Console.WriteLine("Could not find more than one device to togglet between. Did you type the names correct?");
                Console.ReadKey();
                Environment.Exit(0);
            }
            var currentDefaultDevice = relevantDevicesArr.FirstOrDefault(o => o.IsDefaultDevice);
            var currentIndex = Array.IndexOf(relevantDevicesArr, currentDefaultDevice);
            CoreAudioDevice nextDevice = currentIndex == relevantDevicesArr.Length - 1
                ? relevantDevicesArr.First()
                : relevantDevicesArr[currentIndex + 1];
            nextDevice.SetAsDefault();
        }

        private void Initialize() {
            if (!File.Exists(_iniPath)) {
                using (var stream = File.CreateText(_iniPath)) {
                    stream.WriteLine("{0}=A,B,C", DevicesIniKey);
                }
                Console.WriteLine("A {0} has been created for you, fill it with the playback devices you want to toggle between.", IniFileName);
                Console.WriteLine("Path to the .ini: {0}", _iniPath);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public CoreAudioController Controller = new CoreAudioController();
    }
}
