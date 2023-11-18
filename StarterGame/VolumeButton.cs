using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace StarterGame
{
    class VolumeButton
    {
        private const int width = 71;
        private const int height = 70;
        public bool mute;
        public Rectangle rect;
        public Vector2 position;
        public Vector2 spriteSize;
        Vector2 buttonPosition = Vector2.Zero;
        float lerpFactor = 0.5f;
        public VolumeButton()
        {
            mute = false;
            rect = new Rectangle(1100, 20, width, height);
        }

        public void CheckToggle(AudioManager audioManager)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                if (!mute)
                {
                    mute = true;
                    audioManager.PauseBackgroundMusic();
                    return;
                }
                else
                {
                    audioManager.PlayBackgroundMusic();
                    mute = false;
                    return;
                }
            }


            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            if (rect.Contains(mousePoint))
            {
                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (!mute)
                    {
                        mute = true;
                        audioManager.PauseBackgroundMusic();
                        return;
                    }
                    else
                        audioManager.PlayBackgroundMusic();
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

        public void UpdatePosition(int cameraPositionX, int cameraPositionY)
        {
            Vector2 targetButtonPosition = new Vector2(cameraPositionX, cameraPositionY);
            buttonPosition = Vector2.Lerp(buttonPosition, targetButtonPosition, lerpFactor);
            this.rect.X = (int)buttonPosition.X;
            this.rect.Y = (int)buttonPosition.Y;
        }
    }
}
