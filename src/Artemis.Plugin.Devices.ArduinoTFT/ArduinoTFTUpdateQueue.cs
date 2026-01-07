using System;
using System.Threading;
using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftUpdateQueue :
        IUpdateQueue,
        IUpdateQueue<object, Color>,
        IReferenceCounting
    {
        private readonly System.IO.Ports.SerialPort _serialPort;
        private int _referenceCount;

        public ArduinoTftUpdateQueue(System.IO.Ports.SerialPort serialPort)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
        }

        public bool RequiresFlush => false;

        public int ActiveReferenceCount => _referenceCount;

        public void AddReferencingObject(object obj)
            => Interlocked.Increment(ref _referenceCount);

        public void RemoveReferencingObject(object obj)
            => Interlocked.Decrement(ref _referenceCount);

        public void Reset()
        {
        }

        public void Dispose()
        {
        }

        public void SetData(ReadOnlySpan<(object, Color)> dataSet)
        {
            if (!_serialPort.IsOpen)
                return;

            byte[] buffer = new byte[dataSet.Length * 3];
            int i = 0;

            foreach (var entry in dataSet)
            {
                Color c = entry.Item2; // floats 0–1

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
                // ignore transient serial errors
            }
        }
    }
}
