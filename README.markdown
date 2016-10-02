### Welcome

The XDMessaging library provides an easy-to-use, zero configuration solution to inter-process communication for .NET applications. It provides a simple API for broadcasting and receiving messages across application domain, process, and even network boundaries.

The library allows the use of user-defined pseudo channels through which messages may be sent and received. Any application can send a message to any channel, but it must register as a listener with the channel in order to receive. In this way developers can quickly and programmatically devise how their applications will communicate with each other best to work in harmony.

The XDMessaging library comes in 2 flavours. The full version provides the option to use Amazon Queues for sending and receiving messages to remote machines. It also supports network propagtion more which broadcasts messages to processes on remote machines as well as the local machine. 

The XDMessaging.Lite version is for same box communication only, and is therefore is much more lightweight.

### Installation

Install the full version of the library using Nuget.

	PM> Install-Package XDMessaging
	
Install the Lite version of the library using Nuget.

	PM> Install-Package XDMessaging.Lite
	
### User Guide	

Refer the documentation [here](http://thecodeking.github.com/XDMessaging.Net/ "XDMessaging.Net User Guide").