using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using SharedMemory;
using CefShared.Event;

namespace CefServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Do not execute this application!");

                return;
            }

            CefSettings settings = new CefSettings();
            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            string ourInstance = InstanceManager.CreateInstance();

            Console.WriteLine("Spawning new instance ({0})", ourInstance);

            InstanceManager.GetInstance(ourInstance).Width = 1920;
            InstanceManager.GetInstance(ourInstance).Height = 1080;
            InstanceManager.GetInstance(ourInstance).Start();

            Console.ReadLine();
        }
    }
}
