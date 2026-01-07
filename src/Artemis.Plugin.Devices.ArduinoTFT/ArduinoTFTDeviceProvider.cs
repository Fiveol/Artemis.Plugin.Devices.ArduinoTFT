using System;
using System.IO.Ports;
using System.Threading.Tasks;
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
                _serialPort = new SerialPort("COM4", 1000000)
                {
                    ReadTimeout = 2000,
                    WriteTimeout = 2000
                };
                _serialPort.Open();

                _logger.Information("Arduino TFT: Serial port COM4 opened");

                if (!PerformHandshake())
                {
                    _logger.Error("Arduino TFT: Handshake failed, disabling provider");
                    Disable();
                    return;
                }

                _logger.Information("Arduino TFT: Handshake successful");

                ArduinoTftRgbDeviceProvider.Instance.AttachSerialPort(_serialPort);
                ArduinoTftRgbDeviceProvider.Instance.SetReady();
                ArduinoTftRgbDeviceProvider.Instance.Initialize(RGBDeviceType.All, false);

                _deviceService.AddDeviceProvider(this);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to initialize Arduino TFT provider");
                Disable();
            }
        }

        private bool PerformHandshake()
        {
            try
            {
                _serialPort!.Write(new byte[] { 0x01 }, 0, 1);
                _logger.Information("Arduino TFT: Sent handshake byte 0x01");

                int response = _serialPort.ReadByte();

                if (response == 0x01)
                {
                    _logger.Information("Arduino TFT: Received handshake response 0x01");
                    return true;
                }

                _logger.Error($"Arduino TFT: Unexpected handshake response: 0x{response:X2}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Arduino TFT: Handshake exception");
                return false;
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
                catch { }

                _serialPort.Dispose();
                _serialPort = null;
            }

            ArduinoTftRgbDeviceProvider.Instance.Dispose();
        }
    }
}