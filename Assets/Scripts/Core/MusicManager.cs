using UnityEngine;

namespace SignalRoguelite
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource menuSource;
        [SerializeField] private AudioSource battleSource;

        [Header("Tracks")]
        [SerializeField] private AudioClip menuTrack;
        [SerializeField] private AudioClip battleTrack;

        private void Awake()
        {
            // Singleton, чтобы музыка не дублировалась
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Настраиваем источники
            if (menuSource == null) menuSource = gameObject.AddComponent<AudioSource>();
            if (battleSource == null) battleSource = gameObject.AddComponent<AudioSource>();

            menuSource.loop = true;
            menuSource.playOnAwake = false;
            menuSource.clip = menuTrack;

            battleSource.loop = true;
            battleSource.playOnAwake = false;
            battleSource.clip = battleTrack;
        }

        private void Start()
        {
            PlayMenuMusic();
        }

        public void PlayMenuMusic()
        {
            if (battleSource.isPlaying) battleSource.Stop();
            if (!menuSource.isPlaying && menuTrack != null)
                menuSource.Play();
        }

        public void PlayBattleMusic()
        {
            if (menuSource.isPlaying) menuSource.Stop();
            if (!battleSource.isPlaying && battleTrack != null)
                battleSource.Play();
        }

        public void StopAllMusic()
        {
            menuSource.Stop();
            battleSource.Stop();
        }
    }
}