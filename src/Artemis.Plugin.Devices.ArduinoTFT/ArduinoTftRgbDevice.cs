using System;
using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftRgbDevice : AbstractRGBDevice<ArduinoTftDeviceInfo>
    {
        private const int WIDTH = 32;
        private const int HEIGHT = 24;

        public ArduinoTftRgbDevice(ArduinoTftUpdateQueue queue)
            : base(new ArduinoTftDeviceInfo(), queue)
        {
            InitializeLeds();
        }

        private void InitializeLeds()
        {
            int index = 0; // 0 -> LedMatrix1

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    LedId ledId = (LedId)((int)LedId.LedMatrix1 + index);

                    AddLed(
                        ledId,
                        new Point(x, y),
                        new Size(1, 1),
                        customData: (x, y)
                    );

                    index++;
                }
            }
        }
    }
}
