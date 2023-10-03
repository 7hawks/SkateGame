using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class Menu
    {
        public Rectangle startButton;
        public Rectangle exitButton;

        public Menu()
        {
            startButton = new Rectangle(Game1.view.width/2 - 70, 430, Utilities.Scale(59, 3.75), Utilities.Scale(27, 3.75));
            exitButton = new Rectangle(Game1.view.width/2 - 70, 600, Utilities.Scale(59, 3.75), Utilities.Scale(27, 3.75));
        }

        public bool StartCheck()
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (startButton.Contains(mousePoint))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ExitCheck()
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (exitButton.Contains(mousePoint))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
