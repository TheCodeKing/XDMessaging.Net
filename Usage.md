---
layout: page
title: Usage
tagline: quick start
group: navigation
---
{% include JB/setup %}

To begin create an instance of the `XDMessagingCient` as follows:

	// Create XDMessagingClient instance
	XDMessagingClient client = new XDMessagingClient();

#### Broadcast	
	
To send messages use the client to create an instance of `IXDBroadcaster` for a particular transport mode. Use the instance to then broadcast messages on a named channel. A channel is an arbitrary string chosen to represent a channel and are not case sensitive.

	// Create broadcaster instance using HighPerformanceUI mode
	IXDBroadcaster broadcaster = client.Broadcasters
		.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI);
	
	// Send a shutdown message on the commands channel
	broadcaster.SendToChannel("commands", "shutdown");

#### Listen
	
To receive messages use the client to create an instance of `IXDListener` for a particular transport mode. Use the instance to register a channel to listen on.

	// Create our listener instance using HighPerformanceUI mode
	IXDListener listener = client.Listeners
		.GetListenerForMode(XDTransportMode.HighPerformanceUI);
	
	// Register channel to listen on
	listener.RegisterChannel("commands");
	
#### Handle Response	
	
To handle messages received by the listener, attach a `MessageReceived` handler. The `DataGram` contains the original message, and channel name.
	
	// Attach event handler for incoming messages
	listener.MessageReceived += (o,e) {
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
	