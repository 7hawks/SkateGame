using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class Menu
    {
        public Button startButton;
        public Rectangle exitButton;
        private float scale = 3.75f;
        private float width = 59;
        private float height = 32;
        // public var viewport;
        public int viewWidth;
        public static int viewportWidth;
        public bool startButtonContains = false;

        public Menu(GraphicsDevice graphicsDevice, (int width, int height) view)
        {
            var viewport = graphicsDevice.Viewport;

            float widthScaled = width * scale;
            float heightScaled = height * scale;

           // var spriteWidth = this.Content.Load<Texture2D>("./startMenu/titleCard").Width * scale;
         //   var spriteHeight = this.Content.Load<Texture2D>("./startMenu/titleCard").Height * Scale;

            var screenCenterX = viewport.Width / 2;
            var screenCenterY = viewport.Height / 2;

            var spritePosition = new Vector2(screenCenterX - (widthScaled / 2), screenCenterY - (heightScaled / 2));

            viewWidth = view.width;
            startButton = new Button((viewport.Width / 2) - (int)(width * scale / 2), 400, Utilities.Scale(59, scale), Utilities.Scale(27, scale));
           // startButton = new Rectangle(viewport.Width/ 2, 430, Utilities.Scale(59, scale), Utilities.Scale(27, scale));
            exitButton = new Rectangle((viewport.Width / 2) - ((int)widthScaled / 2), 600, Utilities.Scale(59, scale), Utilities.Scale(27, scale));
        }

/*        public bool StartCheck()
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (startButton.ButtonCheck())
            {
                startButtonContains = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
            }
            startButtonContains = false;
            return false;
        }*/

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
