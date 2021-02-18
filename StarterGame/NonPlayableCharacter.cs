using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class NonPlayableCharacter
    {
        private double interval = 250;
        public int frame = 0;
        private double timer = 0f;
        public Rectangle rect;
        public bool mute;
        public Vector2 scale;
        public bool dialogue;
        public Popup popup;
        public Texture2D logo { get; set; }

        public NonPlayableCharacter(Texture2D inputLogo, int locX, int locY, int inputWidth, int inputHeight)
        {
            popup = new Popup(0, 0, "hello there!");
            mute = false;
            logo = inputLogo;
            rect = new Rectangle(locX, locY, inputWidth, inputHeight);
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
