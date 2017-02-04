# XDMessaging

The XDMessaging library provides an easy-to-use, zero configuration solution to inter-process communication for .NET applications. It provides a simple API for broadcasting and receiving messages across application domain, process, and even network boundaries.

The library allows the use of user-defined pseudo channels through which messages may be sent and received. Any application can send a message to any channel, but it must register as a listener with the channel in order to receive. In this way developers can quickly and programmatically devise how their applications will communicate with each other best to work in harmony.

The XDMessaging library offers some advantages over other IPC technologies like WCF, .Net Remoting, Sockets, NamedPipes and MailSlots. To begin with the library does not require a server-client relationship as there is no physical connection between processes.

With XDMessaging messages can be broadcast by multiple applications and instantly received by multiple listeners in a disconnected fashion. It’s also worth noting that most of the existing IPC implementations require the opening of specific ports and somewhat painful configuration of settings to make work. With XDMessaging there is no configuration, the API determines where messages are sent, and which messages are received using pseudo channels.

## Installation

Install the Full version of the library using Nuget. Provides the option to use Amazon Queues for sending and receiving messages to remote machines. It also supports network propagtion mode which broadcasts messages to processes on remote machines as well as the local machine. 

	PM> Install-Package XDMessaging
	
Install the Lite version of the library using Nuget. This version is for same box communication only, and is therefore much more lightweight.

	PM> Install-Package XDMessaging.Lite
	
## Transport Modes

Three different transport modes are supported, with a common API. Developers may switch transport mode at any time.
- **Windows Messaging** (`HighPerformanceUI`)
  - Uses the `WM_COPYDATA` Windows Message to copy data between applications. The broadcaster implementation sends the Windows Messages directly to a hidden window on the listener instance, which dispatches the MessageReceived event with the copied data.
  - Listeners in this mode must be created on the UI Thread.
  - Channels are created by adding/removing Windows properties.
  - This offers the most performant solution for Windows Forms based applications, but does not work for Windows Services, Console apps, or other applications without a message pump.
  
- **File I/O** (`Compatibility`)
  - Uses file I/O to broadcast messages via a shared directory.
  - A FileSystemWatcher is used within listener classes to monitor changes and trigger the MessageReceived event containing the broadcast message.
  - This mode can be used in Windows Services, console applications, and Windows Forms based applications.
  - Channels are created as separate directories on the file system for each channel. The temporary directories should be accessible by all processes, and there should be no need for manual configuration.
  
- **Amazon Web Services** (`RemoteNetwork`)
  - Uses AWS to implement a subscriber/publisher implementation for broadcasting messages over a network and interprocess.
  - There may be associated costs involved in using this mode, and you will need to supply valid Amazon account credentials.
  - This mode is used internally to send messages from other transport modes over the network when using network propagation mode

## API

To begin create an instance of the `XDMessagingCient`:

```csharp
// Create XDMessagingClient instance
XDMessagingClient client = new XDMessagingClient();
```

To send messages use the client to create an instance of `IXDBroadcaster` for a particular transport mode. Use the instance to broadcast messages on a named channel. A channel is an arbitrary string chosen to represent a channel and are not case sensitive.

```csharp
// Create broadcaster instance using HighPerformanceUI mode
IXDBroadcaster broadcaster = client.Broadcasters
    .GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);

// Send a shutdown message on the commands channel
broadcaster.SendToChannel("commands", "shutdown");
```

To receive messages use the client to create an instance of `IXDListener` for a particular transport mode. Use the instance to register a channel to listen on.

```csharp
// Create listener instance using HighPerformanceUI mode
IXDListener listener = client.Listeners
    .GetListenerForMode(XDTransportMode.HighPerformanceUI);

// Register channel to listen on
listener.RegisterChannel("commands");
```

To handle messages received by the listener, attach a `MessageReceived` event handler. The `DataGram` contains the original message and channel name.

```csharp
// Attach event handler for incoming messages
listener.MessageReceived += (o,e) => {
    // e.DataGram.Message is the message
    // e.DataGram.Channel is the channel name
    if (e.DataGram.Channel == "commands")
    {
       switch(e.DataGram.Message)
       {
	case "shutdown":
	   this.Close();
	   break;
       }
    }
}
```
	
For advanced uses cases see [this guide](http://thecodeking.co.uk/project/xdmessaging/advanced/).
