using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarterGame
{
    
    public class CollissionObject
    {
        private int width;
        private int height;
        public Rectangle bounds;
        public bool mute;
        public Vector2 scale;
        public Texture2D logo { get; set; }

        public CollissionObject(Texture2D inputLogo, PlatformType inputType)
        {
            mute = false;
            logo = inputLogo;
            bounds = new Rectangle(662, 200, width, height);
        }

        public void HandleCollision(CollissionObject otherObject)
        {

        }
    }
}
