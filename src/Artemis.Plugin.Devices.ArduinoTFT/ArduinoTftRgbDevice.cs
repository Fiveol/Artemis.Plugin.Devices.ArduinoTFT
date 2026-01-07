using System;
using RGB.NET.Core;

namespace Artemis.Plugin.Devices.ArduinoTFT
{
    public class ArduinoTftRgbDevice : AbstractRGBDevice<ArduinoTftDeviceInfo>
    {
        private const int WIDTH = 32;
        private const int HEIGHT = 24;
        private const int LED_SIZE = 8;

        public ArduinoTftRgbDevice(ArduinoTftUpdateQueue queue)
            : base(new ArduinoTftDeviceInfo(), queue)
        {
            InitializeLeds();
        }

        private void InitializeLeds()
        {
            int index = 0;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    LedId ledId = (LedId)((int)LedId.LedMatrix1 + index);

                    AddLed(
                        ledId,
                        new Point(x * LED_SIZE, y * LED_SIZE),
                        new Size(LED_SIZE, LED_SIZE),
                        customData: (x, y)
                    );

                    index++;
                }
            }
        }
    }
}