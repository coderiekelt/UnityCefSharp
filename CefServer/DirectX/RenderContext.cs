using SharpDX.Direct3D11;

namespace CefServer.DirectX
{
    public static class RenderContext
    {
        public static Device DxDevice;

        public static void Initialize()
        {
            DxDevice = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport, new[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 });

            CreateTexture(1920, 1080);
        }

        public static Texture2D CreateTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(DxDevice, new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.UnorderedAccess,
                CpuAccessFlags = CpuAccessFlags.Read,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.Shared,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Dynamic,
            });

            return texture;
        }
    }
}
