# Audio - Default Device Switcher
A simple program that toggles the default audio playback device on windows.

# How to use
- Compile using Visual Studio.
- Place the produced "AudioDefaultDeviceSwitcher.exe" in the directory you want to run it from.
- Run "AudioDefaultDeviceSwitcher.exe". A "Audio.ini" will be created in the same directory.
- Edit the "Audio.ini" to your devices that you want to toggle between (Separate each name with a comma).
- Run "AudioDefaultDeviceSwitcher.exe" every time you want to toggle between the devices.

# Setup

Example Audio.ini:  
`Devices=Speakers,Realtek HD Audio 2nd output`

Corresponding Playback devices in Windows:  
![Devices](/DeviceImage.png?raw=true "Devices")