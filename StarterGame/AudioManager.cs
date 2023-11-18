using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace StarterGame
{
    public class AudioManager
    {
        public static SoundEffect wreckSound;
        private Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private SoundEffectInstance grindInstance;
        private SoundEffectInstance landingInstance;
        private SoundEffectInstance wreckInstance;
        private SoundEffectInstance popInstance;
        private Song backgroundMusic;

        public AudioManager(Microsoft.Xna.Framework.Content.ContentManager content)
        {
           // landingSound = Content.Load<SoundEffect>("realSkateLand");
            soundEffects.Add("landingSound", content.Load<SoundEffect>("realSkateLand"));
            soundEffects.Add("grindSound", content.Load<SoundEffect>("./audio/grindSound"));
            soundEffects.Add("wreckSound", content.Load<SoundEffect>("./audio/wreckSound"));
            wreckInstance = soundEffects["wreckSound"].CreateInstance();
            landingInstance = soundEffects["landingSound"].CreateInstance();
            soundEffects.Add("popSound", content.Load<SoundEffect>("realSkatePop"));
            popInstance = soundEffects["popSound"].CreateInstance();
            grindInstance = soundEffects["grindSound"].CreateInstance();

            backgroundMusic = content.Load<Song>("./audio/JanouTouryuumon");
            // wreckSound = content.Load<SoundEffect>("./audio/wreckSound");
        }

        public void PlayGrindSound()
        {
            grindInstance.Play();
        }

        public void PlayLandingSound()
        {
            landingInstance.Play();
        }
        public void PlayWreckSound()
        {
            wreckInstance.Play();
        }

        public void PlayPopSound()
        {
            popInstance.Play();
        }

        public void PlayBackgroundMusic()
        {
            MediaPlayer.Play(backgroundMusic);
        }

        public void PauseBackgroundMusic()
        {
            MediaPlayer.Pause();
        }

    }
}
