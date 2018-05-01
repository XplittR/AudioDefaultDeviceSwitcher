using AudioSwitcher.AudioApi.CoreAudio;
using SKYPE4COMLib;

namespace AudioDefaultDeviceSwitcher {
    public class SkypeFixer {
        private readonly CoreAudioDevice _device;
        private static Skype _skype;
        public SkypeFixer(CoreAudioDevice device) {
            _device = device;
            _skype  = new Skype();
        }

        public void SetAudioOut() {
            if (IsLoggedIn()) {
                _skype.Settings.AudioOut = _device.FullName;
            }
        }

        private bool IsLoggedIn() {
            return _skype.Client.IsRunning && _skype.CurrentUserStatus != TUserStatus.cusLoggedOut;
        }
    }
}