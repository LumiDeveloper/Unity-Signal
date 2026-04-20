using UnityEngine;
using System.Collections;

namespace SignalRoguelite
{
    public class BootLoader : MonoBehaviour
    {
        [Header("Windows")]
        [SerializeField] private GameObject loadingWindow;
        [SerializeField] private GameObject fightWindow;
        [SerializeField] private GameObject startButton;

        [Header("Settings")]
        [SerializeField] private float loadingTime = 5f;

        private void Start()
        {
            ResetToMainMonitor();
        }

        private void ResetToMainMonitor()
        {
            if (loadingWindow != null) loadingWindow.SetActive(false);
            if (fightWindow != null) fightWindow.SetActive(false);
            if (startButton != null) startButton.SetActive(true);
        }

        public void OnStartButtonPressed()
        {
            if (startButton != null) startButton.SetActive(false);
            if (loadingWindow != null) loadingWindow.SetActive(true);
            StartCoroutine(LoadGameCoroutine());
        }

        private IEnumerator LoadGameCoroutine()
        {
            yield return new WaitForSeconds(loadingTime);
            
            if (loadingWindow != null) loadingWindow.SetActive(false);
            if (fightWindow != null) fightWindow.SetActive(true);
            
            // Включаем боевую музыку
            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayBattleMusic();
        }

        public void ReturnToMainMonitor()
        {
            ResetToMainMonitor();
            
            // Включаем музыку меню
            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayMenuMusic();
        }
    }
}