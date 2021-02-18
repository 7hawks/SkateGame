using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    
    public class CollissionObject
    {
        private const int width = 266;
        private const int height = 446;
        public Rectangle block;
      //  float targetX = 128;
        public PlatformType platformType;
        public bool mute;
        public Vector2 scale;
        public Texture2D logo { get; set; }

        public CollissionObject(Texture2D inputLogo, PlatformType inputType)
        {
            this.platformType = inputType;
            mute = false;
            logo = inputLogo;
           // block = new Rectangle(262, 200, width, height);
            block = new Rectangle(662, 200, width, height);

            //scale = new Vector2(targetX / (float)logo.Width, targetX / (float)logo.Width);
        }
    }
}
