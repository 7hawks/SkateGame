using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class Wreck
    {
        private const int endFrame = 13;
        private const double interval = 100;
        private double timer;
        public int frame { get; set; }

        public Wreck()
        {
            timer = 0;
            frame = 0;
        }

        public void Animate(double elapsedTime)
        {
            timer += elapsedTime;

            if (timer > interval)
            {
                if (frame == 2)
                {
                    frame += 2;
                }
                frame++;
                timer = 0f;
            }
        }

        public State WreckCheck(State playerState, double elapsedTime, Player player)
        {
            if (frame > endFrame)
            {
               // player.width = 20;
                frame = 0;

                if (player.busted)
                {
                    player.ResetHard();
                }
                else
                {
                    player.Reset();
                    return State.Grounded;
                }
            }
            if (playerState != State.Wreck && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                return State.Wreck;
            }
            else if (playerState == State.Wreck)
            {
                
                Animate(elapsedTime);
                return State.Wreck;
            }

            return playerState;
        }
    }
}
