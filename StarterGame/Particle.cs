using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StarterGame
{
    public class Particle
    {
        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector2 Position { get; set; }        // The current position of the particle        
        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance
        public float Angle { get; set; }            // The current angle of rotation of the particle
        public float AngularVelocity { get; set; }    // The speed that the angle is changing
        public Color Color { get; set; }            // The color of the particle
        public float Size { get; set; }                // The size of the particle
        public int TTL { get; set; }                // The 'time to live' of the particle


        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

    }
}
