# OpenFin Notifications Service Client

The .NET Notifications Service client provides an interface for the [Notifications Service](https://github.com/HadoukenIO/notifications-service) allowing notifications to be manipulated from .NET Applications.

## Dependencies

[OpenFin.Notifications](https://www.nuget.org/packages/OpenFin.Notifications/)
[OpenFin Desktop](https://www.nuget.org/packages/OpenfinDesktop/) (v10.0 and higher)

# Getting Started

## Initialization

The Notification Service API is centered around the static NotificationClient object.
The first call to initialize the client should be to `NotificationClient.Initialize()` to connect to the provider and register the `ChannelClient` and topics to respond to events triggered by the provider. The `NotificationClient.OnInit` action delegate needs to be handled and set before calling `Initialize()` as follows:

```js
NotificationClient.OnInit => { ...} ;

NotificationClient.Initialize();
```

## Notification requirements

- Notifications must have an Id, title, category, and icon

- Notification bodies can either be plain text or markdown 

- Notifications can have at most 4 buttons

## Notification Action Delegates

NotificiationClient action delegates facilitate the handling of various notification related events.

## NotificationActionOccured Delegate

The handler for notiifcation actions are invoked when notification buttons and or bodies are clicked or when notifications expire.

## NotificationClosed Delegate

The handler for these events are invoked when notifications are closed.

## NotificationCreated Delegate

Handlers for these events are invoked when notifications are created.

# Demo Application

![embed](ss1.png)

![embed](ss2.png)

The OpenFin.Notifications.Demo project is a WPF application demonstrating the notification client's functionality. Functionality demonstrated include creating notifications, deleting notifications, creating expiring notifications, configuring buttons within the notifications, responding to events when notifications are interacted with (body and button clicks) or expire, and toggling the visibility of the Notification Center.

To use the provider hosted in our production environment, the build configuration should be set to **Release** before the application is built and ran.

To  debug the test application, you will need to [clone](https://github.com/hadoukenio/notifications-service) and start the notifications service before starting the .NET client debugger. To build the service you will need [Node/NPM](https://nodejs.org).  Once node is installed and the repo has been cloned, navigate to the root directory of the service code and issue the following commands:

- `npm install`

- `npm start` 

## License

The code in this repository is distributed under the Apache License, Version 2.0

However, if you run this code, it may call on the OpenFin RVM or OpenFin Runtime, which are covered by OpenFin’s Developer, Community, and Enterprise licenses. You can learn more about OpenFin licensing at the links listed below or just email us at support@openfin.co with questions.



Copyright 2018-2019 OpenFin

https://openfin.co/developer-agreement/ 
https://openfin.co/licensing/
