# CrossHaptics: Enabling Real-time Multi-device Haptic Feedback for VR Games via Controller Vibration Pattern Analysis
![Teaser Figure](https://i.imgur.com/T3jK4ZA.png)

## Overview
We presented CrossHaptics, which explores how controller vibration patterns designed by game developers can be used to enable support for additional haptic devices, proposed a framework that automatically detects asymmetrical and symmetrical vibration signals to infer localized vs. full-body haptic events in real-time and provided a plugin architecture to simplify adding support for additional haptic devices.


# How To Use 
1. clone the project from github
    ```
    git clone https://github.com/goolu0623/CrossHaptics.git
    ```
1. [install redis](https://redis.io/docs/getting-started/installation/) and start the server.

    <img src="https://i.imgur.com/rHxfc72.png" width="700">

1. Open CrossHaptics.sln with Visual Studio
 

1. Restore NuGet Packages (if needed)

    <img src="https://i.imgur.com/xvu1SBr.png" width="700">

1. Rebuild solution (Rebuild All)

    <img src="https://i.imgur.com/eL7tjVz.png" width="700">
1. Connect all your devices
    1. VR devices
    1. Hatpic devices(sample)


1. Start the scripts and enjoy the game!

# How To Listen To Events With Your Own Haptic Devices Plugin.
## redis installation
[redis installation instructions](https://redis.io/docs/getting-started/installation/)


# For C# Developer
1. add project 
    1. RedisEndpoint_donetFramework if you are using .NET framework
    1. RedisEndpoint_donetCore if you are using .NET core

    <img src="https://i.imgur.com/safzr2c.png" width="700">
1. add RedisEndpoint as reference

    <img src="https://i.imgur.com/vhFqTvv.png" width="700">
1. install StackExchange.REDIS in NUGET manage store

    <img src="https://i.imgur.com/tb86GxK.png" width="700">
1. listen to the channel on local host 6379/6380(or whatever the redis port you select) as the example below
    1. channel name = "symmetrical_event"
    2. channel name = "nonsymmetrical_event"
1. and you'll recieved the event msg in the following format whenever the controllers accept haptic events in real-time


1. sample haptic device implementation useing bHaptics Devices.
```
using System;
using System.IO;
using bHapticsLib;
using System.Threading;
using RedisEndpoint;


namespace SampleHapticDevicePlugin
{
    public class Program
    {
        private ScaleOption scale_option = new ScaleOption();
        private HapticPattern hapticFeedback;
        
        private void Main(){
            // bHaptics device pattern initialization
            string hapticFeedbackPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "hapticFeedbackPattern.tact");
            hapticFeedback = HapticPattern.LoadFromFile("testfeedback", hapticFeedbackPath);            
            bHapticsManager.Connect("bHapticsLib", "TestApplication", maxRetries: 0);
            Thread.Sleep(1000);

            // bHaptics device connection initialization 
            bHapticsConnection Connection = new bHapticsConnection("bHapticsLib", "AdditionalConnection", maxRetries: 0);
            Connection.BeginInit();
            Console.WriteLine(Connection.Status);

            // Subscribe redics channel
            Subscriber exampleSubscriber = new Subscriber("localhost", 6379);
            string subscribeChannelName = "nonsymmetrical_event"; // here subscribe to symmetrical_event or nonsymmectrical_event
            exampleSubscriber.SubscribeTo(subscribeChannelName);
            exampleSubscriber.msgQueue.OnMessage(msg => msgHandler(msg.Message));

            Console.Read();
            return;
        }

        void msgHandler(string msg){
            // msg example as below
            // 06:43:52.024 RightController Output Vibration Amp 0.1600 Freq 1.0000 Duration 0.0000
            // seperate the information you need
            string [] EventMessage = msg.Split(' ');
            string  Amp = EventMessage[5];
            string  Dur = EventMessage[9];
            
            // play aroudn with your device here
            scale_option.Duration = Convert.ToSingle(Dur);
            scale_option.Intensity = Convert.ToSingle(Amp);
            hapticFeedback.Play(scale_option);
        }
    }
}
```
# For Unity Developer
1. [install NugetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) in your Unity project
1. restart your Unity project
1. install StackExchange.Redis in Unity with NugetForUnity

    <img src="https://i.imgur.com/agVdmL6.png" width="700">
1. Make sure if the version of StackExchange you install in Unity Porject is the same as the version of CrossHapticsStarter(check in NuGetManager)
    
    <img src="https://i.imgur.com/MGaMKQd.png" width="700">
1. if Not, update stackExchange and relevent package to the same version.
1. Add class RedisEndpoint to Unity script for ease of use (you may skip this step if you want to setup your own Redis connection)
    ```
    using StackExchange.Redis;
    public class RedisEndpoint {

        protected static ConfigurationOptions redisConfiguration;
        protected static ConnectionMultiplexer multiplexer;
        protected ISubscriber connection;

        public RedisEndpoint(string url, ushort port) {
            if (multiplexer == null) {
                string host = url + ":" + port.ToString();
                string redisConfiguration2 = host+"Password=password,Ssl=false,ConnectTimeout=6000,SyncTimeout=6000,AllowAdmin=true";
                multiplexer = ConnectionMultiplexer.Connect(redisConfiguration2);
            }
        }

        static int Main() {
            return 0;
        }
    }

    public class Publisher : RedisEndpoint {
        public Publisher(string url, ushort port) : base(url, port) {
            connection = multiplexer.GetSubscriber();
        }
        public void Publish(string channelName, string msg) {
            connection.PublishAsync(channelName, msg, flags: CommandFlags.FireAndForget);
        }
    }

    public class Subscriber : RedisEndpoint {
        public ChannelMessageQueue msgQueue;
        public Subscriber(string url, ushort port) : base(url, port) {
            connection = multiplexer.GetSubscriber();
        }
        public void SubscribeTo(string channelName) {
            msgQueue = connection.Subscribe(channelName);
        }
    }
    ```
1. Listen to the events as the following sample usage to connect with your device
    ```
    using UnityEngine;

    public class redis_code_sample : MonoBehaviour
    {
        Subscriber subscriber;
        string testChannelName = "test";
        string url = "localhost";
        ushort port = 6379;
        void Start()
        {
            subscriber = new Subscriber(url, port);
            subscriber.SubscribeTo(testChannelName);
            subscriber.msgQueue.OnMessage(msg => msgHandler(msg.Message));
        }

        // Update is called once per frame
        void Update()
        {
        }

        void msgHandler(string message) {
            // msg example as below
            // 06:43:52.024 RightController Output Vibration Amp 0.1600 Freq 1.0000 Duration 0.0000
            // seperate the information you need
            string [] EventMessage = msg.Split(' ');
            string  Amp = EventMessage[5];
            string  Dur = EventMessage[9];
            
            // play around with your device here

        }
    }

    ```
