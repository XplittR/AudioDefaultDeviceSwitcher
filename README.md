# Audio - Default Device Switcher
A simple program that toggles the default audio playback device on windows.

# How to use
1. Retrieve the AudioDefaultDeviceSwitcher.exe.
  * Option 1: Compile using Visual Studio, and place the produced "AudioDefaultDeviceSwitcher.exe" in the directory you want to run it from.
  * Option 2: Download the [latest release](https://github.com/XplittR/AudioDefaultDeviceSwitcher/releases) and place the .exe in the directory you want to run it from.  
2. Run "AudioDefaultDeviceSwitcher.exe". A "Audio.ini" will be created in the same directory.  
3. Edit the "Audio.ini" to your devices that you want to toggle between (Separate each name with a comma).  
4. Run "AudioDefaultDeviceSwitcher.exe" every time you want to toggle between the devices.  

# Setup
Example Audio.ini:  

    Devices=Speakers,Realtek HD Audio 2nd output
    AlsoSetCommunications=true

Corresponding Playback devices in Windows:  
![Devices](/DeviceImage.png?raw=true "Devices")


## Issues?
If you encounter any issues, feature requests or pull requests, please do not hesitate to submit them!

## Thanks
- Thanks to [xenolightning](https://github.com/xenolightning) for creating [AudioSwitcher API](https://github.com/xenolightning/AudioSwitcher).
- Thanks to [Fody](https://github.com/Fody) for creating [Costura.Fody](https://github.com/Fody/Costura).
