using CefShared.Memory;
using CefSharp;
using CefSharp.Enums;
using CefSharp.OffScreen;
using CefSharp.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CefServer.Chromium
{
    class CefRenderHandler : IRenderHandler
    {
        public int Width;
        public int Height;

        public int ViewX;
        public int ViewY;

        public MemoryInstance GfxMemory;

        public CefRenderHandler(MemoryInstance gfxMemory, int width, int height)
        {
            GfxMemory = gfxMemory;
            Width = width;
            Height = height;
        }

        public void Dispose()
        {
            
        }

        public ScreenInfo? GetScreenInfo()
        {
            return null;
        }

        public bool GetScreenPoint(int viewX, int viewY, out int screenX, out int screenY)
        {
            screenX = ViewX;
            screenY = ViewY;

            return true;
        }

        public Rect GetViewRect()
        {
            return new Rect(ViewX, ViewY, Width, Height);
        }

        public void OnAcceleratedPaint(PaintElementType type, Rect dirtyRect, IntPtr sharedHandle)
        {
        }

        public void OnCursorChange(IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
        {
            
        }

        public void OnImeCompositionRangeChanged(CefSharp.Structs.Range selectedRange, Rect[] characterBounds)
        {
            
        }

        public void OnPaint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            byte[] internalBuffer = new byte[width * height * 4];

            Marshal.Copy(buffer, internalBuffer, 0, internalBuffer.Length);

            GfxMemory.WriteBytes(internalBuffer);
        }

        public void ResetViewport()
        {
            ViewX = 0;
            ViewY = 0;
        }

        public void OnPopupShow(bool show)
        {
            
        }

        public void OnPopupSize(Rect rect)
        {
            
        }

        public void OnVirtualKeyboardRequested(IBrowser browser, TextInputMode inputMode)
        {
            
        }

        public bool StartDragging(IDragData dragData, DragOperationsMask mask, int x, int y)
        {
            return false;
        }

        public void UpdateDragCursor(DragOperationsMask operation)
        {
           
        }
    }
}
