using System.Globalization;
using System.IO;
using System.Linq;

namespace AudioDefaultDeviceSwitcher {
    public class EasyIni {
        //"manual" ini-file handling to avoid extra dependencies
        private readonly string[] _settings;

        public EasyIni(string filename) {
            _settings = File.ReadAllLines(filename);
        }

        public string Val(string key, string defaultVal = null) {
            var set = _settings.FirstOrDefault(s => s.StartsWith(key + "=", true, CultureInfo.CurrentCulture));
            if (string.IsNullOrWhiteSpace(set))
                return defaultVal;
            return set.Split('=')[1];
        }

        public bool Val(string key, bool defaultVal) {
            var set = _settings.FirstOrDefault(s => s.StartsWith(key + "=", true, CultureInfo.CurrentCulture));
            if (string.IsNullOrWhiteSpace(set))
                return defaultVal;
            return bool.Parse(set.Split('=')[1]);
        }
    }
}