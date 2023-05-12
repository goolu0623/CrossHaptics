# CrossHaptics: Enabling Real-time Multi-device Haptic Feedback for VR Games via Controller Vibration Pattern Analysis
![Teaser ver24](https://github.com/goolu0623/CrossHaptics/assets/69243118/f1b1d81d-d925-4439-b277-cbf6dfd47f1d)

## Overview
We presented CrossHaptics, which explores how controller vibration patterns designed by game developers can be used to enable support for additional haptic devices, proposed a framework that automatically detects asymmetrical and symmetrical vibration signals to infer localized vs. full-body haptic events in real-time and provided a plugin architecture to simplify adding support for additional haptic devices.


## How To Use
1. git clone https://github.com/goolu0623/CrossHaptics.git
1. Open GameSolution.sln with Visual Studio
1. Restore NuGet Packages (if needed)
1. Rebuild solution (Rebuild All)
1. Connect all your devices (including VR devices and haptic devices)
1. start the scripts and enjoy the game!

## How To Listen To Events With Your Own Haptic Devices Plugin.
1. install redis and start the server.
1. listen to the channel on local host
  1. channel name = "symmetrical_event"
  2. channel name = "nonsymmetrical_event"
1. and you'll get the event in the following format.
1. 

