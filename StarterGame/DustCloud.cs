using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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



        public DustCloud(int inputWidth, int inputHeight)
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
            if (frame > lifeTime)
            {
                frame = 0;
                timer = 0;
                active = false;
            }

            Animate(elapsedTime);

            if (frame > lifeTime)
            {
                active = false;
                frame = 0;
            }
            return new Rectangle(frame * 28, 0, 28, 15);
        }


    }
}
