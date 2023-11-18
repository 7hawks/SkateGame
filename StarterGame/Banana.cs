using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace StarterGame
{
    public class Banana : GameObject
    {

        private Texture2D logo;
        public int frame = 0;

        public Banana(Texture2D inputLogo, int x, int y, int width, int height) : base(x, y, width, height)
        {
            logo = inputLogo;
        }

        public Rectangle Sprite()
        {
            return new Rectangle(frame * 16, 0, 16, 16);
        }
    }
}
