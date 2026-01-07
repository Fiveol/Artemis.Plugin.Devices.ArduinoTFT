using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftDeviceInfo : IRGBDeviceInfo
    {
        public RGBDeviceType DeviceType { get; set; } = RGBDeviceType.Unknown;

        public string DeviceName { get; set; } = "Arduino TFT 32x24";

        public string Manufacturer { get; set; } = "Arduino";

        public string Model { get; set; } = "MCUFriend TFT";

        public object? LayoutMetadata { get; set; } = null;
    }
}
