using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    class VolumeButton
    {
        private const int width = 71;
        private const int height = 70;
        public bool mute;
        public Rectangle rect;

        public VolumeButton()
        {
            mute = false;
            rect = new Rectangle(1100, 20, width, height);
        }

        public void CheckToggle(Song song)
        {
            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (rect.Contains(mousePoint))
            {
                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!mute)
                    {
                        mute = true;
                        MediaPlayer.Pause();
                        return;
                    }
                    else
                        MediaPlayer.Play(song);
                        mute = false;
                }
            }
        }

        public Rectangle Sprite()
        {
            if (this.mute)
            {
                return new Rectangle(21, 0, 21, 19);
            }
            else
                return new Rectangle(0, 0, 21, 19);
        }
    }
}
