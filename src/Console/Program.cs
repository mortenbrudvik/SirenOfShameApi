using System.Threading.Tasks;
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
            
            sirenOfShame.PlayLight(LightSignal.SOS);
            sirenOfShame.PlayAudio();
            
            await Task.Delay(10000);
            
            sirenOfShame.StopAudio();
            sirenOfShame.StopLight();
            sirenOfShame.Disconnect();
        }
    }
}
