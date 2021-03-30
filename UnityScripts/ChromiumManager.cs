using System.Diagnostics;
using System.Threading;
using UnityEngine;
public class ChromiumManager : MonoBehaviour
{
    public int EventServerPort = 9002;

    private Process _process;
    private bool _running = false;

    void Awake()
    {
        string cefServerPath;

        #if UNITY_EDITOR
            cefServerPath = Application.dataPath + @"\..\CefServer";
        #endif

        if (cefServerPath == null) {
            cefServerPath = System.Reflection.Assembly.GetExecutingAssembly().Location + @"\..\..\CefServer";
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
            Thread.Sleep(100);

            _running = true;
            CefClient.InstanceManager instanceManager = gameObject.AddComponent<CefClient.InstanceManager>();
            instanceManager.EventServerPort = EventServerPort;

            instanceManager.Initialize();
        }
    }

    void OnDestroy()
    {
        if (_running) {
            _process.Kill();
        }
    }
}
