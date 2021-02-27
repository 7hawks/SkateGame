using Microsoft.Xna.Framework.Graphics;

namespace StarterGame
{
    public class Platform : GameObject
    {
        public PlatformType type { get; set; }
        public const int height = 50;
        public Texture2D sprite;
        public bool intersects;

        public Platform(Texture2D sprite, int x, int y, int width, int height, PlatformType inputType) : base( x, y, width, height )
        {
            intersects = false;
            this.sprite = sprite;
            type = inputType;
        }
    }
}
