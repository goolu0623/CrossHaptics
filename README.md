# CrossHaptics: Enabling Real-time Multi-device Haptic Feedback for VR Games via Controller Vibration Pattern Analysis
![Teaser ver24](https://github.com/goolu0623/CrossHaptics/assets/69243118/f1b1d81d-d925-4439-b277-cbf6dfd47f1d)

## Overview
We presented CrossHaptics, which explores how controller vibration patterns designed by game developers can be used to enable support for additional haptic devices, proposed a framework that automatically detects asymmetrical and symmetrical vibration signals to infer localized vs. full-body haptic events in real-time and provided a plugin architecture to simplify adding support for additional haptic devices.


## How To Use
1. git clone https://github.com/goolu0623/CrossHaptics.git
1. Open GameSolution.sln with Visual Studio
1. install redis and start the server.
1. Restore NuGet Packages (if needed)
1. Rebuild solution (Rebuild All)
1. Connect all your devices (including VR devices and haptic devices)
1. start the scripts and enjoy the game!

## How To Listen To Events With Your Own Haptic Devices Plugin.
### redis installation
### C# .net framework 4.6.1
1. add project RedisEndpoint_donetFramework as reference
1. listen to the channel on local host 6379/6380(or whatever the redis port you select) as the example below
  1. channel name = "symmetrical_event"
  2. channel name = "nonsymmetrical_event"
1. and you'll get the event in the following format.
1. 
```
using RedisEndpoint;
void sample(){
  Subscriber exampleSubscriber = new Subscriber("localhost", 6379);
  string subscribeChannelName = "symmetrical_event"; \\ here subscribe to symmetrical_event or nonsymmectrical_event
  exampleSubscriber.SubscribeTo(subscribeChannelName);
  exampleSubscribe.msgQueue.OnMessage(msg=>msgHandler(msg.Message));
}
void msgHandler(string msg){
  \\ msg example as below
  \\ 06:43:52.024  :RightController|Output|Vibration|Amp 0.1600 Freq 1.0000 Duration 0.0000
  \\ seperate the information you need


  \\ play your device here
  
}
```
### python
### arduino

