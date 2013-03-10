---
layout: page 
title: Advanced
tagline: beyond basics
group: navigation
---
{% include JB/setup %}

Transport implementations are loosely coupled to the core library, and it's not necessary to create project references to the transport assemblies themselves. In order to use specific implementations, the appropriate assembly must be placed in the application installation directory.

### Enable RemoteNetwork

The default implementation of `RemoteNetwork` mode uses `Amazon Web Services`, specifically SQS & SNS. The library will create instances of these services on demand provided valid AWS credentials are provided. Refer to the Amazon documentation with regard to any associated cost implications in using these services. The credentials issued by your Amazon account can be specified in the `app.config` file as follows.

	<?xml version="1.0"?>
	<configuration>
	  <appSettings>
		<add key="AWSAccessKey" value="#accesskey#"/>
		<add key="AWSSecretKey" value="#accesssecret#"/>
	  </appSettings>
	</configuration>
	
Alternatively by adding a reference to the `XDMessaging.Transport.Amazon.dll` assembly it's possible to define these programmatically. Additionally it's possible to then set the desired AWS region.

	XDMessagingClient client = new XDMessagingClient()
		.WithAmazonSettings(accessKey, accessSecret, RegionEndPoint.EUWest1);

If using multiple applications with conflicting channel names, it's possible to additionally partition AWS messages by a unique name. This may be useful if deploying the same application to multiple test environments. In this case messages are only send and received to applications with the same AWS settings.

	XDMessagingClient client = new XDMessagingClient()
		.WithAmazonUniqueKey("qa");

### Network Propagation

Network propagation is a feature of the library that allows messages to leverage `HighPerformanceUI` or `Compatibility` modes, whilst additionally distributing messages to a remote server. This uses `RemoteNetwork` mode under the hood, and messages are re-broadcast on the remote server using the original transport mode.

In order to enable `NetworkPropagation` an additional flag is set when creating the `IXDBroadcaster` instance.

	// Create instance of HighPerformanceUI broadcaster and enable network propagation
	IXDBroadcaster broadcaster = client.Broadcasters
		.GetBroadcasterForMode(XDTransportMode.HighPerformanceUI, true);

### Explicit Modes

If project references are added to the transport assemblies, then extension methods are made available on `XDMessagingClient` which strong type the specific transport implementations (rather than relying on defaults).

	// Create listener instance using WindowsMessaging mode (HighPerformaceUI)
	IXDListener listener = client.Listeners.GetWindowsMessagingListener();
	
	// Create listener instance using IOStream mode (Compatibility)
	IXDListener listener = client.Listeners.GetIoStreamListener();
	
	// Create listener instance using Amazon mode (RemoteNetwork)
	IXDListener listener = client.Listeners.GetAmazonListener();
	
	// Create broadcaster instance using WindowsMessaging mode (HighPerformaceUI)
	IXDBroadcaster broadcaster = client.Broadcasters.GetWindowsMessagingBroadcaster();
	
	// Create broadcaster instance using IOStream mode (Compatibility)
	IXDBroadcaster broadcaster = client.Broadcasters.GetIoStreamListener();
	
	// Create broadcaster instance using Amazon mode (RemoteNetwork)
	IXDBroadcaster broadcaster = client.Broadcasters.GetAmazonListener();

