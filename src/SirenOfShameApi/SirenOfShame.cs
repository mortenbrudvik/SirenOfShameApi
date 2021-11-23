using System;
using System.Linq;
using SirenOfShameApi.SirenDevice;

namespace SirenOfShameApi
{
    /// <summary>
    /// Provides a simplified interface to the Siren of Shame API.
    /// https://github.com/AutomatedArchitecture/sirenofshame-uwp-device-api
    /// </summary>
    public class SirenOfShame 
    {
        private readonly SirenOfShameDevice _sirenOfShameDevice = new();
        private UsbInfoPacket _deviceInfo;
        
        public SirenStatus Status
        {
            get
            {
                var deviceInfo = _sirenOfShameDevice.ReadDeviceInfo().Result;
                var ledPatternName = _sirenOfShameDevice.LedPatterns.FirstOrDefault(x => x.Id == deviceInfo.LedMode)?.Name;
                return new SirenStatus(deviceInfo.LedMode + "", ledPatternName, deviceInfo.LedPlayDuration);
            }
        }

        // Connect to the Siren of Shame device, and starts listening if device is connected to computer or not.
        public void Connect()
        {
            _sirenOfShameDevice.Connected += SirenOfShameDeviceOnConnected;
            _sirenOfShameDevice.Disconnected += SirenOfShameDeviceOnDisconnected;
            _sirenOfShameDevice.StartWatching();
        }

        public void Disconnect()
        {
            _sirenOfShameDevice.Connected -= SirenOfShameDeviceOnConnected;
            _sirenOfShameDevice.Disconnected -= SirenOfShameDeviceOnDisconnected;
            _sirenOfShameDevice.Dispose();
        }

        // intensity is 0-6 for all patterns except SOS 
        public void PlayLight(LightPattern pattern, uint intensity = 0, uint durationInSeconds = 0)
        {
            var lightPattern = GetLightPattern(pattern, Math.Min(intensity, 6));
            Console.Out.WriteLineAsync($"Playing light pattern \"{lightPattern.Name}\", Id:{lightPattern.Id}");
            _sirenOfShameDevice.PlayLightPattern(lightPattern, durationInSeconds > 0 ?  TimeSpan.FromSeconds(durationInSeconds) : null);
            
            Console.Out.WriteLineAsync($"Led mode: {_deviceInfo.LedMode}, duration: {_deviceInfo.LedPlayDuration}");
            Console.Out.WriteLineAsync($"Audio mode: {_deviceInfo.AudioMode}, duration: {_deviceInfo.AudioPlayDuration}");
        }

        private LedPattern GetLightPattern(LightPattern pattern, uint intensity = 0) =>
            pattern switch
            {
                LightPattern.SOS => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 2),
                LightPattern.SlowPulse => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 3 + intensity),
                LightPattern.Pulse => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 10 + intensity),
                LightPattern.Chase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 17 + intensity),
                LightPattern.DarkChase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 24 + intensity),
                LightPattern.FadeChase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 31 + intensity),
                LightPattern.RandomFades => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 38 + intensity),
                LightPattern.Random => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 45 + intensity),
                LightPattern.JarringFlash => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 52 + intensity),
                LightPattern.Dim => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 59 + intensity),
                LightPattern.MaxByte => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 66 + intensity),
                _ =>  _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=> x.Id == 1) // Off
            };
        
        public void StopLight()
        {
            var p = GetLightPattern(LightPattern.Off);
            _sirenOfShameDevice.PlayLightPattern(p, null).Wait();
        }

        private async void SirenOfShameDeviceOnConnected(object? sender, EventArgs e)
        {
            _deviceInfo = await _sirenOfShameDevice.ReadDeviceInfo();

            await Console.Out.WriteLineAsync("Siren of Shame have connected.");

            foreach (var pattern in _sirenOfShameDevice.LedPatterns)
            {
               // await System.Console.Out.WriteLineAsync($"Pattern \"{pattern.Name}\", Id:{pattern.Id}");
            }
        }

        private void SirenOfShameDeviceOnDisconnected(object? sender, EventArgs e)
        {
            Console.Out.WriteLine("Siren of shame have disconnected");
        }
    }
    
    public record SirenStatus(string LightModeId, string LightModeName, uint LightDuration);
}