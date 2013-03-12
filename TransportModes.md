---
layout: page
title: Transport Modes
tagline: pros & cons
group: navigation
weight: 2
---
{% include JB/setup %}

Transport Modes define the implementation to use when broadcasting and receiving messages. There are three modes as follows, each with advantages over the other. Modes are abstracted from their specific implementations for extensibility, and loosely coupled. In order to use one of the modes the appropriate assembly must be placed in the application execution directory.

### Compatibility

The default implementation uses file based IO to broadcast messages via a shared directory. A `FileSystemWatcher` is used within listener classes to monitor changes and trigger the `MessageReceived` event containing the broadcast message. This mode can be used in Windows Services, console applications, and Windows Forms based applications. Channels are created as separate directories on the file system for each channel. The temporary directories should be accessible by all processes, and there should be no need for manual configuration. To use this implementation add a reference to `XDMessaging.Transport.IOStream` in your project.

### HighPerformanceUI

The default implementation uses the `WM_COPYDATA` Windows Message to copy data between applications. The broadcaster implementation sends the Windows Messages directly to a hidden window on the listener instance, which dispatches the MessageReceived event with the copied data. Channels are created by adding/removing Windows properties. This offers the most performant solution for Windows Forms based applications, but does not work for Windows Services, Console apps, or other applications without a message pump. To use this implementation add a reference to `XDMessaging.Transport.WindowsMessaging` in your project.

### RemoteNetwork

The default implementation uses `Amazon Web Services` to implement a subscriber/publisher implementation for broadcasting messages over a network and interprocess. There may be associated costs involved in using this mode, and you will need to supply valid Amazon account credentials. This mode is used internally to send messages from other transport modes over the network when using network propagation mode. To use this implementation add a reference to `XDMessaging.Transport.Amazon` in your project.