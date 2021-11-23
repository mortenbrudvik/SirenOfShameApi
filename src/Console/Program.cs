﻿using System.Threading.Tasks;
using SirenOfShameApi;

namespace Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sirenOfShame = new SirenOfShame();
            sirenOfShame.Connect();

            await Task.Delay(1000);
            
            sirenOfShame.PlayLight(LightPattern.SOS);
            
            await Task.Delay(10000);
            
            sirenOfShame.StopLight();
            sirenOfShame.Disconnect();
        }
    }
}