using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    class TestData
    {
        private SpriteBatch spriteBatch;
        public void PrintTestData(SpriteBatch newSpriteBatch, SpriteFont font, Player player1, VolumeButton volume, ParticleEngine particleEngine, Vector2 cameraPosition, Menu menu, Viewport viewport, SecurityGuard guard)
        {
            spriteBatch = newSpriteBatch;

            var mouseState = Mouse.GetState();
            var mousePoint = new Point(mouseState.X, mouseState.Y);

            spriteBatch.DrawString(font, "busted: " + player1.busted, new Vector2(cameraPosition.X, cameraPosition.Y + 220), Color.Black);
            spriteBatch.DrawString(font, "emitter location Y: " + particleEngine.EmitterLocation.Y, new Vector2(cameraPosition.X, cameraPosition.Y + 260), Color.Black);
            spriteBatch.DrawString(font, "player rect Y: " + player1.rect.Y, new Vector2(cameraPosition.X, cameraPosition.Y + 300), Color.Black);
            spriteBatch.DrawString(font, "emitter location X: " + particleEngine.EmitterLocation.X, new Vector2(cameraPosition.X, cameraPosition.Y + 360), Color.Black);
            spriteBatch.DrawString(font, "player rect X: " + player1.rect.X, new Vector2(cameraPosition.X, cameraPosition.Y + 400), Color.Black);

            spriteBatch.DrawString(font, "guard position x: " + guard.position.X, new Vector2(cameraPosition.X, cameraPosition.Y + 460), Color.Black);
            spriteBatch.DrawString(font, "guard position y: " + guard.position.Y, new Vector2(cameraPosition.X, cameraPosition.Y + 420), Color.Black);
            spriteBatch.DrawString(font, "elapsed time: " + guard.elapsedTime, new Vector2(cameraPosition.X, cameraPosition.Y + 500), Color.Black);
           
            spriteBatch.DrawString(font, "elapsed time: " + guard.elapsedTime, new Vector2(cameraPosition.X, cameraPosition.Y + 570), Color.Black);
           
            spriteBatch.DrawString(font, "Start button X: " + menu.startButton.button.X, new Vector2(cameraPosition.X, cameraPosition.Y + 640), Color.Black);
            spriteBatch.DrawString(font, "transform X: " + guard.transformX, new Vector2(cameraPosition.X, cameraPosition.Y + 680), Color.Black);
            spriteBatch.DrawString(font, "transform Y: " + guard.transformY, new Vector2(cameraPosition.X, cameraPosition.Y + 720), Color.Black);
            spriteBatch.DrawString(font, "menu viewWidth: " + menu.viewWidth, new Vector2(cameraPosition.X, cameraPosition.Y + 760), Color.Black);
           // spriteBatch.DrawString(font, "start button contains: " + menu.startButton.Contains(), new Vector2(cameraPosition.X, cameraPosition.Y + 800), Color.Black);
            spriteBatch.DrawString(font, "guard velocity X: " + guard.velocity.X, new Vector2(cameraPosition.X, cameraPosition.Y + 800), Color.Black);
            spriteBatch.DrawString(font, "guard velocity Y: " + guard.velocity.Y, new Vector2(cameraPosition.X, cameraPosition.Y + 840), Color.Black);
            
            spriteBatch.DrawString(font, "security guard state: " + guard.CurrentState, new Vector2(cameraPosition.X, cameraPosition.Y + 920), Color.Black);
            spriteBatch.DrawString(font, "guard X: " + guard.rect.X, new Vector2(cameraPosition.X, cameraPosition.Y + 960), Color.Black);

        }
    }
}
