---
layout: page
title: Welcome
tagline: about the library
---
{% include JB/setup %}

The XDMessaging library provides an easy-to-use, zero configuration solution to same-box communications. It provides a simple API for broadcasting and receiving messages across application domain, process, and even network boundaries.

The library allows the use of user-defined pseudo channels through which messages may be sent and received. Any application can send a message to any channel, but it must register as a listener with the channel in order to receive. In this way developers can quickly and programmatically devise how their applications will communicate with each other best to work in harmony.

The messages may optionally be propagated to other processes over a network automatically.

## Installation

Install the library using Nuget.

> PM> Install-Package XDMessaging