using System;
using System.IO.Ports;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RGB.NET.Core;
using Serilog;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    [PluginFeature(Name = "Arduino TFT Device Provider")]
    public class ArduinoTFTDeviceProvider : DeviceProvider
    {
        private readonly ILogger _logger;
        private readonly IDeviceService _deviceService;
        private SerialPort? _serialPort;

        public ArduinoTFTDeviceProvider(ILogger logger, IDeviceService deviceService)
        {
            _logger = logger;
            _deviceService = deviceService;
        }

        public override IRGBDeviceProvider RgbDeviceProvider => ArduinoTftRgbDeviceProvider.Instance;

        public override void Enable()
        {
            try
            {
                _serialPort = new SerialPort("COM4", 115200);
                _serialPort.Open();

                ArduinoTftRgbDeviceProvider.Instance.AttachSerialPort(_serialPort);
                ArduinoTftRgbDeviceProvider.Instance.Initialize(RGBDeviceType.All, false);

                _deviceService.AddDeviceProvider(this);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to initialize Arduino TFT provider");
                Disable();
            }
        }

        public override void Disable()
        {
            _deviceService.RemoveDeviceProvider(this);

            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                        _serialPort.Close();
                }
                catch
                {
                }

                _serialPort.Dispose();
                _serialPort = null;
            }

            ArduinoTftRgbDeviceProvider.Instance.Dispose();
        }
    }
}
