using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class ChromiumManager : MonoBehaviour
{
    public int EventServerPort = 9002;

    public bool UseInternalProcess = true;

    private Process _process;
    private bool _running = false;

    void Awake()
    {
        string cefServerPath = null;

        if (!UseInternalProcess) {
            CefClient.InstanceManager instanceManager = gameObject.AddComponent<CefClient.InstanceManager>();
            instanceManager.EventServerPort = EventServerPort;

            instanceManager.Initialize();
            DontDestroyOnLoad(instanceManager);

            return;
        }

        #if UNITY_EDITOR
            cefServerPath = Application.dataPath + @"\..\CefServer";
        #endif

        if (cefServerPath == null) {
            cefServerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\..\..\CefServer";
        }

        
        _process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                WorkingDirectory = cefServerPath,
                FileName = cefServerPath + @"\CefServer.exe",
                Arguments = EventServerPort.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        if (_process.Start()) {
            Thread.Sleep(1000);

            _running = true;
            CefClient.InstanceManager instanceManager = gameObject.AddComponent<CefClient.InstanceManager>();
            instanceManager.EventServerPort = EventServerPort;

            instanceManager.Initialize();
            DontDestroyOnLoad(instanceManager);
        }
    }

    void OnDestroy()
    {
        if (_running) {
            _process.Kill();
        }
    }
}
