using Microsoft.Xna.Framework;

namespace StarterGame
{
    public class DustCloud
    {
        public bool active;
        public int lifeTime = 4;
        public int frame;
        public int x;
        public int y;

        private const double interval = 80;
        private double timer;

        public Rectangle rect;

        public DustCloud()
        {
            frame = 0;
        }


        public void Animate(double elapsedTime)
        {
            timer += elapsedTime;

            if (timer > interval)
            {
                frame++;
                timer = 0f;
            }
        }

        public void Draw(int inputX, int inputY)
        {
            rect = new Rectangle(inputX, inputY + 80, Utilities.Scale(28, 3.75), Utilities.Scale(15, 3.75));
            x = inputX;
            y = inputY;
            active = true;
        }

        public Rectangle Update(double elapsedTime)
        {
            if (frame > lifeTime) // dustcloud is done so return basic Rectangle
            {
                frame = 0;
                timer = 0;
                active = false;
                return new Rectangle(); 
            }

            Animate(elapsedTime);

            return new Rectangle(frame * 28, 0, 28, 15);
        }
    }
}
