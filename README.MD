# UnityCefSharp

**This project is under heavy development, and not production ready!**

UnityCefSharp is an implementation of CefSharp aimed at Unity games. It utilizes an external process with mapped memory files and TCP to facilitate communication between Unity and the external process.

The reason for starting a new project is the lack of well documented and maintained alternatives, and a lack of UI frameworks for Unity that I like (a.k.a. are free.)

You can see this UI framework in action in the game [Era Roleplay](http://era-roleplay.com).

## Projects within this solution
### CefServer
This is the actual server application, and is running in the background to serve the CefClient.

### CefShared
Contains shared classes, such as events and interprocess memory management.

### CefClient
Contains everything you need to implement this within Unity.

## Building
Copy both `PostBuildServer.bat.dist` and `PostBuildClient.bat.dist` and remove the `.dist` part.

Change `targetDir` to your desired target directory.

Launch the solution, and wait for the NuGet packages to install.

Simply click "Build Solution" and enjoy!

## Best practices
It's important to remember that in Unity, anything happening related to Unity objects needs to happen in the main thread.

I have made some efforts to keep the FPS bearable by off-loading the actual loading and copying of the texture memory to a thread.
Note that the renderer will run at a target framerate of 24 FPS by default, you can alter this by setting `CefInstance.TargetFPS` to your desired framerate. Be careful though, higher target = higher burden on the CPU!

If there is someone smarter than me out there that can implement a shared texture resource (with e.g. the native pointer of a texture) feel free to let me know!

Until that happens, be mindful of using this in your project. During my tests it takes around 3 to 5ms to generate a single frame (full stop, since the last update) on a resolution 2162x814 (in editor.)

I recommend only using this for your main GUI, but hey I'm just a README.MD not a cop so do what you want.

## Getting started
See [Getting Started](https://github.com/coderiekelt/UnityCefSharp/wiki/Getting-started)