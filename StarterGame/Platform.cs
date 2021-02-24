using Microsoft.Xna.Framework;

namespace StarterGame
{
    public class Platform
    {
        public Rectangle rect;
        public PlatformType type { get; set; }
        public const int height = 50;

        public Platform(int x, int y, int height, int width, PlatformType inputType)
        {
            rect = new Rectangle(x, y, height, width);
            type = inputType;
        }
    }
}
