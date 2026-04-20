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
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int currentHP = 100;
        [SerializeField] private int attackBonus = 0;
        [SerializeField] private int healBonus = 0;
        
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
        
        // Events
        public event Action OnPlayerDeath;
        public event Action OnRewardSelected;
        
        // Public properties for access from other scripts
        public int MaxHP 
        { 
            get => maxHP; 
            private set 
            { 
                maxHP = Mathf.Clamp(value, 50, 500);
                if (currentHP > maxHP) currentHP = maxHP;
                UpdateUI();
            }
        }
        
        public int CurrentHP 
        { 
            get => currentHP; 
            set 
            { 
                currentHP = Mathf.Clamp(value, 0, maxHP);
                Debug.Log($"[CurrentHP setter] New value: {currentHP}/{maxHP}");
                UpdateUI();
                if (currentHP <= 0) OnPlayerDeath?.Invoke();
            }
        }
        
        public int AttackBonus 
        { 
            get => attackBonus; 
            private set 
            { 
                attackBonus = Mathf.Clamp(value, 0, 100);
                UpdateUI();
            }
        }
        
        public int HealBonus 
        { 
            get => healBonus; 
            private set 
            { 
                healBonus = Mathf.Clamp(value, 0, 100);
                UpdateUI();
            }
        }
        
        void Start()
        {
            Initialize();
        }
        
        void Initialize()
        {
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && levelUpSound != null)
                audioSource = gameObject.AddComponent<AudioSource>();
            
            // Adjust initial values
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            attackBonus = Mathf.Clamp(attackBonus, 0, 100);
            healBonus = Mathf.Clamp(healBonus, 0, 100);
            
            UpdateUI();
        }
        
        public void ShowReward()
        {
            if (rewardPanel == null)
            {
                Debug.LogWarning("[RoguelikeProgress] RewardPanel not assigned!");
                ApplyRandomReward();
                return;
            }
            
            string[] rewards = {
                "+10 HP",
                "+5 Attack",
                "+8 Healing",
                "Restore 20 HP",
                "+3 Attack and +3 Healing",
                "Max HP +15"
            };
            
            int r1 = UnityEngine.Random.Range(0, rewards.Length);
            int r2;
            do { r2 = UnityEngine.Random.Range(0, rewards.Length); } while (r2 == r1);
            
            if (rewardButton1 != null)
            {
                var buttonText = rewardButton1.GetComponentInChildren<TMP_Text>();
                if (buttonText != null) buttonText.text = rewards[r1];
                rewardButton1.onClick.RemoveAllListeners();
                rewardButton1.onClick.AddListener(() => ApplyReward(r1));
            }
            
            if (rewardButton2 != null)
            {
                var buttonText = rewardButton2.GetComponentInChildren<TMP_Text>();
                if (buttonText != null) buttonText.text = rewards[r2];
                rewardButton2.onClick.RemoveAllListeners();
                rewardButton2.onClick.AddListener(() => ApplyReward(r2));
            }
            
            rewardPanel.SetActive(true);
        }
        
        void ApplyReward(int rewardIndex)
        {
            Debug.Log($"[RoguelikeProgress] Applying reward {rewardIndex}");
            
            switch (rewardIndex)
            {
                case 0: // +10 HP
                    CurrentHP = Mathf.Min(currentHP + 10, maxHP);
                    break;
                case 1: // +5 Attack
                    AttackBonus += 5;
                    break;
                case 2: // +8 Healing
                    HealBonus += 8;
                    break;
                case 3: // Restore 20 HP
                    CurrentHP = Mathf.Min(currentHP + 20, maxHP);
                    break;
                case 4: // +3 Attack and +3 Healing
                    AttackBonus += 3;
                    HealBonus += 3;
                    break;
                case 5: // Max HP +15
                    MaxHP += 15;
                    CurrentHP = Mathf.Min(currentHP + 15, maxHP);
                    break;
            }
            
            if (rewardPanel != null)
                rewardPanel.SetActive(false);
            
            if (levelUpSound != null && audioSource != null)
                audioSource.PlayOneShot(levelUpSound);
            
            OnRewardSelected?.Invoke();
        }
        
        void ApplyRandomReward()
        {
            int randomReward = UnityEngine.Random.Range(0, 4);
            ApplyReward(randomReward);
        }
        
        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            Debug.Log($"[RoguelikeProgress] TakeDamage: {damage} damage, was {currentHP}");
            CurrentHP -= damage;
        }
        
        public void ForceSetCurrentHP(int newHP)
        {
            currentHP = Mathf.Clamp(newHP, 0, maxHP);
            Debug.Log($"[ForceSetCurrentHP] Set HP = {currentHP}/{maxHP}");
            UpdateUI();
            
            if (currentHP <= 0)
            {
                OnPlayerDeath?.Invoke();
            }
        }
        
        public void ForceKill()
        {
            Debug.Log("[ForceKill] Forcing player death");
            currentHP = 0;
            UpdateUI();
            OnPlayerDeath?.Invoke();
        }
        
        public void Heal(int amount)
        {
            if (amount <= 0) return;
            Debug.Log($"[RoguelikeProgress] Heal: {amount} healing, was {currentHP}");
            CurrentHP += amount;
        }
        
        public void ResetProgress()
        {
            maxHP = 100;
            currentHP = 100;
            attackBonus = 0;
            healBonus = 0;
            UpdateUI();
            Debug.Log("[RoguelikeProgress] Progress reset to initial values");
        }
        
        public void UpdateUI()
        {
            if (hpText != null)
                hpText.text = $"HP: {currentHP}/{maxHP}";
            if (attackBonusText != null)
                attackBonusText.text = $"Attack: +{attackBonus}";
            if (healBonusText != null)
                healBonusText.text = $"Healing: +{healBonus}";
        }
        
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