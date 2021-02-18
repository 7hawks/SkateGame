using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class Coin
    {
        private readonly double interval = 230;
        private double timer = 0f;
        private const int width = 41;
        private const int height = 86;
        public int frame = 0;
        private bool retrieved;


        readonly 
            SoundEffect[] sounds = new SoundEffect[5];

        public Rectangle rect;

        public Coin(int x, int y, SoundEffect[] inputSounds)
        {
            this.sounds = inputSounds;
            retrieved = false;
            rect = new Rectangle(x, y, width, height);
        }

        public Rectangle Sprite()
        {
            if (!retrieved)
            {
                return new Rectangle(frame * 11, 0, 11, 23);
            }
            else 
                return new Rectangle(11 * 11, 23, 11, 23);  // gibberish value to return transparent
        }

        public void AnimateCoin(double elapsedTime, Player player)
        {
            if (!retrieved)
            {
                CheckBounds(player);

                timer += elapsedTime;

                if (timer > interval)
                {
                    frame++;

                    if (!retrieved && frame > 7)
                    {
                        frame = 0;
                    }

                    timer = 0f;
                }
            }

            return;
        }

        private void CheckBounds(Player player)
        {
            if(rect.Intersects(player.player))
            {
                retrieved = true;
                player.coinCount++;
                Random random = new Random();
                int num = random.Next(0, 4);
                sounds[num].Play();
            }
        }
    }
}
