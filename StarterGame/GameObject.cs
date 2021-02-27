

using Microsoft.Xna.Framework;

namespace StarterGame
{
    public class GameObject
    {
        public Rectangle rect;

        public GameObject(int x, int y, int width, int height)
        {
            rect = new Rectangle(x, y, width, height);
        }
    }
}
