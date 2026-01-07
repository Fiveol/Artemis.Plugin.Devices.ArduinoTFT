using System;
using System.Collections.Generic;
using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftRgbDeviceProvider : IRGBDeviceProvider
    {
        private static readonly Lazy<ArduinoTftRgbDeviceProvider> _instance =
            new(() => new ArduinoTftRgbDeviceProvider());

        public static ArduinoTftRgbDeviceProvider Instance => _instance.Value;

        private readonly List<IRGBDevice> _devices = new();
        private readonly List<(int id, IDeviceUpdateTrigger trigger)> _triggers = new();

        private System.IO.Ports.SerialPort? _serialPort;

        public bool IsInitialized { get; private set; }
        public bool ThrowsExceptions { get; private set; }
        public bool Ready { get; private set; }

        public IReadOnlyList<IRGBDevice> Devices => _devices;
        public IReadOnlyList<(int id, IDeviceUpdateTrigger trigger)> UpdateTriggers => _triggers;

        public event EventHandler<ExceptionEventArgs>? Exception;
        public event EventHandler<DevicesChangedEventArgs>? DevicesChanged;

        private ArduinoTftRgbDeviceProvider() { }

        public void AttachSerialPort(System.IO.Ports.SerialPort port)
        {
            _serialPort = port;
        }

        public void SetReady()
        {
            Ready = true;
        }

        public bool Initialize(RGBDeviceType loadFilter = RGBDeviceType.All, bool throwExceptions = false)
        {
            ThrowsExceptions = throwExceptions;
            _devices.Clear();

            if (_serialPort == null)
                return false;

            try
            {
                var queue = new ArduinoTftUpdateQueue(_serialPort);
                var device = new ArduinoTftRgbDevice(queue);
                _devices.Add(device);

                IsInitialized = true;

                DevicesChanged?.Invoke(
                    this,
                    new DevicesChangedEventArgs(device, DevicesChangedEventArgs.DevicesChangedAction.Added)
                );

                return true;
            }
            catch (Exception ex)
            {
                if (ThrowsExceptions)
                    throw;

                Exception?.Invoke(this, new ExceptionEventArgs(ex));
                return false;
            }
        }

        public void Dispose()
        {
            foreach (var d in _devices)
                d.Dispose();

            _devices.Clear();
            IsInitialized = false;
        }
    }
}