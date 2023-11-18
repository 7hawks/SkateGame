using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StarterGame
{
    public class HudManager
    {
        private readonly double interval = 450;
        private double timer = 0f;
        private const int width = 61;
        private const int height = 9;
        public Texture2D kickflip { get; set; }
        public bool kickflipState = false;
        public Rectangle rect;
        private double scale = 3.75;
        public HudManager(Texture2D kickflipImage)
        {
            kickflip = kickflipImage;
            rect = new Rectangle(1100, 500, Utilities.Scale(width, scale), Utilities.Scale(height, scale));
        }

        public void Update(int cameraPositionX, int cameraPositionY, double elapsedTime)
        {
          //  Vector2 targetButtonPosition = new Vector2(cameraPositionX, cameraPositionY);
          //  buttonPosition = Vector2.Lerp(buttonPosition, targetButtonPosition, lerpFactor);
            this.rect.X = cameraPositionX + 400;
            this.rect.Y = cameraPositionY + 600;

            if (kickflipState)
            {
                timer += elapsedTime;
                scale += .05;
                rect.Width = Utilities.Scale(width, scale);
                rect.Height = Utilities.Scale(height, scale);

                if (timer > interval)
                {
                    timer = 0f;
                    kickflipState = false;
                    scale = 3.75;
                }
            }

        }

        public void SetState(bool state)
        {
            kickflipState = state;

        }

        public void DisplayTrickName()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw HUD elements, such as trick name and points
            if (kickflipState)
            {

                if (timer > 400)
                {
                    spriteBatch.Draw(kickflip, this.rect, null, new Color(255, 255, 255, 50), 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    return;
                }
                if (timer > 300)
                {
                    spriteBatch.Draw(kickflip, this.rect, null, new Color(255, 255, 255, 100), 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    return;
                }
                if (timer > 200)
                {
                    spriteBatch.Draw(kickflip, this.rect, null, new Color(255, 255, 255, 180), 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    return;
                }


                spriteBatch.Draw(kickflip, this.rect, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
            }
                
           
        }

    }
}
