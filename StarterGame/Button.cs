using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class Button
    {
        public Rectangle button;
        private int width;
        public int height;
        //  private float scale;
        private bool contains = false;

        public Button(int x, int y, int width, int height)
        {
            button = new Rectangle(x, y, width, height);
               this.width = width;
               this.height = height;
            // this.scale = scale;
        }


        public bool ButtonCheck()
        {
            var mouseState = Mouse.GetState();
            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            if (mouseState.X > button.X && mouseState.X < button.X + button.Width && mouseState.Y > button.Y && mouseState.Y < button.Y + height){
                contains = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
            }


/*            if (button.Contains(mousePoint))
            {
                contains = true;
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    return true;
                }
            }*/
            contains = false;
            return false;
        }

        private void OnMouseDown()
        {
            // Handle button click

        }

        public bool Contains()
        {
            if (this.contains)
            {
                return true;
            }

            else
                return false;

        }

    }
}
