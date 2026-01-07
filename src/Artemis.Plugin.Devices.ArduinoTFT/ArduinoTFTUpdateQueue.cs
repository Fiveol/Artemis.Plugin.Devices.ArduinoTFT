using System;
using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftUpdateQueue :
        IUpdateQueue,
        IUpdateQueue<object, Color>,
        IReferenceCounting
    {
        private readonly System.IO.Ports.SerialPort _serialPort;
        private int _refCount;

        public ArduinoTftUpdateQueue(System.IO.Ports.SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public bool RequiresFlush => false;

        public int ActiveReferenceCount => _refCount;

        public void AddReferencingObject(object obj)
        {
            _refCount++;
        }

        public void RemoveReferencingObject(object obj)
        {
            _refCount--;
        }

        public void Reset() { }

        public void Dispose() { }

        public void SetData(ReadOnlySpan<(object, Color)> dataSet)
        {
            // Do not send LED updates until handshake is complete
            if (!ArduinoTftRgbDeviceProvider.Instance.Ready)
                return;

            if (!_serialPort.IsOpen)
                return;

            byte[] buffer = new byte[dataSet.Length * 5];
            int i = 0;

            foreach (var entry in dataSet)
            {
                var led = (Led)entry.Item1;
                var (x, y) = ((int x, int y))led.CustomData!;

                // 1-based LED ID
                int id = y * 32 + x + 1;

                buffer[i++] = (byte)(id & 0xFF);
                buffer[i++] = (byte)((id >> 8) & 0xFF);

                Color c = entry.Item2;

                buffer[i++] = (byte)(c.R * 255f);
                buffer[i++] = (byte)(c.G * 255f);
                buffer[i++] = (byte)(c.B * 255f);
            }

            try
            {
                _serialPort.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                // Ignore transient serial errors
            }
        }
    }
}