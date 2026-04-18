// LUMI :)
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SignalRoguelite
{
    public class RoguelikeProgress : MonoBehaviour
    {
        [Header("Player Stats")]
        public int maxHP = 100;
        public int currentHP = 100;
        public int attackBonus = 0;
        public int healBonus = 0;
        
        [Header("UI Elements")]
        public GameObject rewardPanel;
        public TMP_Text rewardText1;
        public TMP_Text rewardText2;
        public Button rewardButton1;
        public Button rewardButton2;
        public TMP_Text hpText;
        public TMP_Text attackBonusText;
        public TMP_Text healBonusText;
        
        [Header("Sound")]
        public AudioClip levelUpSound;
        private AudioSource audioSource;
        
        // События, которые могут слушать другие скрипты
        public event Action<int> OnPlayerDamage;
        public event Action<int> OnPlayerHeal;
        public event Action OnPlayerDeath;
        
        void Start()
        {
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
                
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
                
            UpdateUI();
        }
        
        // Вызывается после победы над врагом
        public void ShowReward()
        {
            if (rewardPanel == null)
            {
                Debug.LogWarning("RewardPanel не назначен!");
                ApplyRandomReward();
                return;
            }
            
            rewardPanel.SetActive(true);
            
            string[] rewards = {
                "+10 HP",
                "+5 к атаке",
                "+8 к лечению",
                "Восстановить 20 HP",
                "+3 к атаке и +3 к лечению",
                "Максимальное HP +15"
            };
            
            int r1 = UnityEngine.Random.Range(0, rewards.Length);
            int r2;
            do { r2 = UnityEngine.Random.Range(0, rewards.Length); } while (r2 == r1);
            
            // Обновляем текст на кнопках
            if (rewardButton1 != null)
            {
                var buttonText = rewardButton1.GetComponentInChildren<TMP_Text>();
                if (buttonText != null) buttonText.text = rewards[r1];
            }
            
            if (rewardButton2 != null)
            {
                var buttonText = rewardButton2.GetComponentInChildren<TMP_Text>();
                if (buttonText != null) buttonText.text = rewards[r2];
            }
            
            // ОЧЕНЬ ВАЖНО: очищаем старые слушатели и добавляем новые
            rewardButton1.onClick.RemoveAllListeners();
            rewardButton2.onClick.RemoveAllListeners();
            
            rewardButton1.onClick.AddListener(() => ApplyReward(r1));
            rewardButton2.onClick.AddListener(() => ApplyReward(r2));
        }
        
        void ApplyReward(int rewardIndex)
        {
            Debug.Log($"Применяется награда {rewardIndex}");  // Отладка
            
            switch (rewardIndex)
            {
                case 0:
                    currentHP = Mathf.Min(currentHP + 10, maxHP);
                    break;
                case 1:
                    attackBonus += 5;
                    break;
                case 2:
                    healBonus += 8;
                    break;
                case 3:
                    currentHP = Mathf.Min(currentHP + 20, maxHP);
                    break;
                case 4:
                    attackBonus += 3;
                    healBonus += 3;
                    break;
                case 5:
                    maxHP += 15;
                    currentHP += 15;
                    break;
            }
            
            // Закрываем панель
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
            
            // Обновляем UI
            UpdateUI();
            
            // Звук
            if (levelUpSound != null && audioSource != null)
                audioSource.PlayOneShot(levelUpSound);
            
            OnRewardSelected?.Invoke();
        }

        // Добавь событие в начало класса
        public event System.Action OnRewardSelected;
        
        void ApplyRandomReward()
        {
            int randomReward = UnityEngine.Random.Range(0, 4);
            ApplyReward(randomReward);
        }
        
        public void TakeDamage(int damage)
        {
            currentHP -= damage;
            if (currentHP <= 0)
            {
                currentHP = 0;
                OnPlayerDeath?.Invoke();
            }
            UpdateUI();
        }
        
        public void Heal(int amount)
        {
            currentHP = Mathf.Min(currentHP + amount, maxHP);
            UpdateUI();
            OnPlayerHeal?.Invoke(amount);
        }
        
        public void ResetProgress()
        {
            currentHP = maxHP;
            attackBonus = 0;
            healBonus = 0;
            UpdateUI();
        }
        
        public void UpdateUI()
        {
            if (hpText != null)
                hpText.text = $"HP: {currentHP}/{maxHP}";
            if (attackBonusText != null)
                attackBonusText.text = $"Бонус к атаке: +{attackBonus}";
            if (healBonusText != null)
                healBonusText.text = $"Бонус лечения: +{healBonus}";
        }
        
        // Для доступа из других скриптов
        public int GetModifiedAttack(int baseAttack)
        {
            return baseAttack + attackBonus;
        }
        
        public int GetModifiedHeal(int baseHeal)
        {
            return baseHeal + healBonus;
        }
    }
}