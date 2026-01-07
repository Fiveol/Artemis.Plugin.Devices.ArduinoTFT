#include <MCUFRIEND_kbv.h>
#include <Adafruit_GFX.h>

MCUFRIEND_kbv tft;

#define WIDTH 32
#define HEIGHT 24
#define PIXEL_SIZE 10

bool connected = false;

void setup()
{
    Serial.begin(1000000);

    uint16_t ID = tft.readID();
    tft.begin(ID);
    tft.setRotation(1);

    tft.fillScreen(0x0000); // black on boot
}

void loop()
{
    // Handshake: wait for Artemis to send 0x01
    if (!connected)
    {
        if (Serial.available() > 0)
        {
            uint8_t b = Serial.read();
            if (b == 0x01)
            {
                Serial.write(0x01); // respond
                connected = true;
            }
        }
        return;
    }

    // After handshake, process LED updates
    while (Serial.available() >= 5)
    {
        uint8_t packet[5];
        Serial.readBytes(packet, 5);

        uint16_t id = packet[0] | (packet[1] << 8);

        if (id < 1 || id > 768)
            continue;

        uint16_t index = id - 1;

        uint8_t r = packet[2];
        uint8_t g = packet[3];
        uint8_t b = packet[4];

        int x = index % WIDTH;
        int y = index / WIDTH;

        uint16_t color = tft.color565(r, g, b);

        tft.fillRect(
            x * PIXEL_SIZE,
            y * PIXEL_SIZE,
            PIXEL_SIZE,
            PIXEL_SIZE,
            color
        );
    }
}
