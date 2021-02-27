using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class NonPlayableCharacter : GameObject
    {
        private double interval = 250;
        public int frame = 0;
        private double timer = 0f;
        public bool mute;
        public bool dialogue;
        public Popup popup;
        public Texture2D logo { get; set; }

        public NonPlayableCharacter(Texture2D inputLogo, int x, int y, int width, int height):base(x, y, width, height)
        {
            popup = new Popup(0, 0, "hello there!");
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

        public void CheckPopup()
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (!dialogue && rect.Contains(mousePoint))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    dialogue = true;
                    return;
                }
            }
            if (dialogue)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    dialogue = false;
                }
            }
        }

        public Rectangle Sprite()
        {
             return new Rectangle(frame * 20, 0, 20, 28);
        }
    }
}
