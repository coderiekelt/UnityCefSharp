# UnityCefSharp

**This project is under heavy development, and not production ready!**

UnityCefSharp is an implementation of CefSharp aimed at Unity games. It utilizes an external process with on disk shared memory to facilitate communication between Unity and the external process.

The reason for starting a new project is the lack of well documented and maintained alternatives, and a lack of UI frameworks for Unity that I like (a.k.a. are free.)

You can see this UI framework in action in the game [Era Roleplay](http://era-roleplay.com).

## Projects within this solution
### CefServer
This is the actual server application, and is running in the background to serve the CefClient.

### CefShared
Contains shared classes, such as events and interprocess memory management.

### CefClient
Contains everything you need to implement this within Unity.

