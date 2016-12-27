### Welcome

The XDMessaging library provides an easy-to-use, zero configuration solution to inter-process communication for .NET applications. It provides a simple API for broadcasting and receiving messages across application domain, process, and even network boundaries.

The library allows the use of user-defined pseudo channels through which messages may be sent and received. Any application can send a message to any channel, but it must register as a listener with the channel in order to receive. In this way developers can quickly and programmatically devise how their applications will communicate with each other best to work in harmony.

Three different transport modes are supported, with a common API. Developers may switch transport mode at any time.
- **Windows Messaging** (`HighPerformanceUI`)
  - Uses the `WM_COPYDATA` Windows Message to copy data between applications. The broadcaster implementation sends the Windows Messages directly to a hidden window on the listener instance, which dispatches the MessageReceived event with the copied data.
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

### Installation

Install the Full version of the library using Nuget. Provides the option to use Amazon Queues for sending and receiving messages to remote machines. It also supports network propagtion mode which broadcasts messages to processes on remote machines as well as the local machine. 

	PM> Install-Package XDMessaging
	
Install the Lite version of the library using Nuget. This version is for same box communication only, and is therefore much more lightweight.

	PM> Install-Package XDMessaging.Lite
	
### User Guide	

Refer the documentation [here](http://thecodeking.github.com/XDMessaging.Net/ "XDMessaging.Net User Guide").
