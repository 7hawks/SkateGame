using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarterGame
{
    public class Boombox : GameObject
    {
        private double interval = 250;
        public int frame = 0;
        private double timer = 0f;
        public bool mute;
        public Texture2D logo { get; set; }

        public Boombox(Texture2D inputLogo, int x, int y, int width, int height) : base(x, y, width, height)
        {
            mute = false;
            logo = inputLogo;
        }

        public void Animate(double elapsedTime)
        {

            timer += elapsedTime;

            if (timer > interval)
            {
                frame++;
                if (frame > 3)
                {
                    frame = 0;
                }
                timer = 0f;
            }
        }

        public Rectangle Sprite()
        {
            return new Rectangle(frame * 16, 0, 16, 32);
        }

    }
}
