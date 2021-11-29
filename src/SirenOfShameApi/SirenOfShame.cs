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
        
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        
        public LightPattern LightStatus
        {
            get
            {
                var deviceInfo = _sirenOfShameDevice.ReadDeviceInfo().Result;
                var ledPattern = _sirenOfShameDevice.LedPatterns.FirstOrDefault(x => x.Id == deviceInfo.LedMode);
                return ledPattern == null 
                    ? new LightPattern(1, LightSignal.Off, 0) 
                    : GetLightPattern(ledPattern);
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
            StopLight();
            _sirenOfShameDevice.Dispose();
        }

        // intensity is 0-6 for all patterns except SOS 
        public void PlayLight(LightSignal pattern, uint intensity = 0, uint durationInSeconds = 0)
        {
            var lightPattern = GetLedPattern(pattern, Math.Min(intensity, 6));
            Console.Out.WriteLineAsync($"Playing light pattern \"{lightPattern.Name}\", Id:{lightPattern.Id}");
            _sirenOfShameDevice.PlayLightPattern(lightPattern, durationInSeconds > 0 ?  TimeSpan.FromSeconds(durationInSeconds) : null).Wait();
        }

        private LedPattern GetLedPattern(LightSignal pattern, uint intensity = 0) =>
            pattern switch
            {
                LightSignal.SOS => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 2),
                LightSignal.SlowPulse => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 3 + intensity),
                LightSignal.Pulse => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 10 + intensity),
                LightSignal.Chase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 17 + intensity),
                LightSignal.DarkChase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 24 + intensity),
                LightSignal.FadeChase => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 31 + intensity),
                LightSignal.RandomFades => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 38 + intensity),
                LightSignal.Random => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 45 + intensity),
                LightSignal.JarringFlash => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 52 + intensity),
                LightSignal.Dim => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 59 + intensity),
                LightSignal.MaxByte => _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=>x.Id == 66 + intensity),
                _ =>  _sirenOfShameDevice.LedPatterns.FirstOrDefault(x=> x.Id == 1) // Off
            };

        private static LightPattern GetLightPattern(LedPattern pattern) =>
            pattern.Id switch
            {
                2 => new LightPattern(pattern.Id, LightSignal.SOS, 0),
                >= 3 and <= 3+6 => new LightPattern(pattern.Id, LightSignal.SlowPulse, pattern.Id - 3),
                >= 10 and <= 10+6 => new LightPattern(pattern.Id, LightSignal.Pulse, pattern.Id - 10),
                >= 17 and <= 17+6 => new LightPattern(pattern.Id, LightSignal.Chase, pattern.Id - 17),
                >= 24 and <= 24+6 => new LightPattern(pattern.Id, LightSignal.DarkChase, pattern.Id - 24),
                >= 31 and <= 31+6 => new LightPattern(pattern.Id, LightSignal.FadeChase, pattern.Id - 31),
                >= 38 and <= 38+6 => new LightPattern(pattern.Id, LightSignal.RandomFades, pattern.Id - 38),
                >= 45 and <= 45+6 => new LightPattern(pattern.Id, LightSignal.Random, pattern.Id - 45),
                >= 52 and <= 52+6 => new LightPattern(pattern.Id, LightSignal.JarringFlash, pattern.Id - 52),
                >= 59 and <= 59+6 => new LightPattern(pattern.Id, LightSignal.Dim, pattern.Id - 59),
                >= 66 and <= 66+6 => new LightPattern(pattern.Id, LightSignal.MaxByte, pattern.Id - 66),
                _ => new LightPattern(pattern.Id, LightSignal.Off, 0)
            };
        
        public void StopLight()
        {
            var p = GetLedPattern(LightSignal.Off);
            _sirenOfShameDevice.PlayLightPattern(p, null).Wait();
        }

        private async void SirenOfShameDeviceOnConnected(object? sender, EventArgs e) => Connected?.Invoke(this, EventArgs.Empty);
        private void SirenOfShameDeviceOnDisconnected(object? sender, EventArgs e) => Disconnected?.Invoke(this, EventArgs.Empty);
    }
    
    public record LightPattern(int Id, LightSignal Signal, int Duration);
}