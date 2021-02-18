using Microsoft.Xna.Framework;

namespace StarterGame
{
    public class Platform
    {
        public Rectangle rect;
        public const int height = 50;

        public Platform(int x, int y, int height, int width)
        {
            rect = new Rectangle(x, y, height, width);
        }
    }
}
