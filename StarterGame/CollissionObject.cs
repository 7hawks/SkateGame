using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarterGame
{
    class CollissionObject
    {
        public Rectangle block;
        float targetX = 128;
       // public Vector2 position;
        public Vector2 scale;
        public Texture2D logo { get; set; }

        public CollissionObject(Texture2D inputLogo)
        {
            logo = inputLogo;
            block = new Rectangle(200, 200, 200, 200);
          //  position = new Vector2(200, 200);
            scale = new Vector2(targetX / (float)logo.Width, targetX / (float)logo.Width);
        }
    }
}
