using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StarterGame
{
    public class ParticleEngine
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;
        public Texture2D texture;
        private readonly int totalParticles = 2;

        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
            texture = textures[random.Next(textures.Count)];
        }

        private Particle GenerateNewParticle()
        {
            texture = textures[random.Next(textures.Count)];


            // Generate random velocities for X and Y within a specified range
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));


           // velocity = new Vector2(0f, 0f); // Moves particles horizontally to the right

            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);


/*            Color color = new Color(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble());*/

            // Random size and time-to-live for the particle
            float size = (float)random.NextDouble();
            int ttl = random.Next(20);


            return new Particle(texture, EmitterLocation, velocity, angle, angularVelocity, Color.White, size * 4, ttl);
        }


        public void Update(Player player)
        {
            Vector2 playerPosition = new Vector2(player.rect.X, player.rect.Y + player.heightScaled);
            EmitterLocation = playerPosition;
            int total = totalParticles;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }



        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            for (int index = 0; index < particles.Count; index++)
            {


                spriteBatch.Draw(particles[index].Texture, particles[index].Position, null, particles[index].Color,
                   particles[index].Angle, origin, particles[index].Size, SpriteEffects.None, .02f);
                //  spriteBatch.Draw(particles[index].Texture, particles[index].Position, null, particles[index].Color, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                // spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/exitButton"), , null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
            }
        }

    }
}
