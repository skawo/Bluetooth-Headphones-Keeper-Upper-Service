using System;
using Topshelf;
using System.Media;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System.Threading;

namespace Bluetooth_Headphones_Keeper_Upper_Service
{
    class Program
    {
        public class BluetoothAudioKeeperUpper
        {
            private SoundPlayer Player = null;
            bool AbortThread = false;
            Thread PlayerThread;

            private MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
            private NotificationClientImplementation NCI;
            private IMMNotificationClient NC;

            public BluetoothAudioKeeperUpper()
            {
                NCI = new NotificationClientImplementation();
                NC = NCI;
                deviceEnum.RegisterEndpointNotificationCallback(NC);

                System.IO.Stream Sound = Properties.Res.Silence;
                Player = new SoundPlayer(Sound);
            }

            private void Execute(Object obj)
            {
                while (!AbortThread)
                {
                    if (NCI.DoRestart)
                    {
                        Player.Stop();
                        Player.PlayLooping();
                        NCI.DoRestart = false;
                    }

                    Thread.Sleep(1000);
                }

                Player.Stop();
            }

            internal class NotificationClientImplementation : IMMNotificationClient
            {
                public bool DoRestart = true;
                
                public void OnDeviceAdded(string deviceId) { }

                public void OnDeviceRemoved(string deviceId) { }

                public NotificationClientImplementation() { }

                public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

                public void OnDeviceStateChanged(string deviceId, DeviceState newState) { DoRestart = true; }

                public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { DoRestart = true; }
            }

            public void Start() { AbortThread = false; PlayerThread = new Thread(Execute); PlayerThread.Start(); }
            public void Stop() { AbortThread = true; PlayerThread.Join(); }

        }

        public static void Main()
        {
            var rc = HostFactory.Run(x =>                                  
            {
                x.Service<BluetoothAudioKeeperUpper>(s =>                                   
                {
                    s.ConstructUsing(name => new BluetoothAudioKeeperUpper());                
                    s.WhenStarted(tc => tc.Start());                        
                    s.WhenStopped(tc => tc.Stop());                         
                });
                x.RunAsLocalSystem();                                      

                x.SetDescription("Service which keeps Bluetooth headphones always on, to prevent the audio startup lag problem.");                  
                x.SetDisplayName("Bluetooth Heaphones Keeper-Upper Service");                                 
                x.SetServiceName("Bluetooth Heaphones Keeper-Upper Service");                                 
            });                                                             

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  
            Environment.ExitCode = exitCode;
        }
    }
}
