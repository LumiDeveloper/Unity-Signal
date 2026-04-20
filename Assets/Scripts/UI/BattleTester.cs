// LUMI :)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace SignalRoguelite
{
    public class BattleTester : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Dropdown color1Dropdown;
        [SerializeField] private TMP_Dropdown color2Dropdown;
        [SerializeField] private TMP_Dropdown color3Dropdown;
        [SerializeField] private TMP_Dropdown shape1Dropdown;
        [SerializeField] private TMP_Dropdown shape2Dropdown;
        [SerializeField] private TMP_Dropdown shape3Dropdown;     
        [SerializeField] private TMP_Text battleLogText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button nextLevelButton;

        [Header("Scout")]
        [SerializeField] private GameObject scoutPanel;
        [SerializeField] private TMP_Text scoutEnemyNameText;
        [SerializeField] private TMP_Text scoutShapesText;
        [SerializeField] private TMP_Text scoutTurnOrderText;
        [SerializeField] private Button scoutButton;

        [Header("Level Counter")]
        [SerializeField] private TMP_Text levelCounterText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverScoreText;
        [SerializeField] private Button restartButton;

<<<<<<< HEAD
        [Header("Color Selectors")]
        [SerializeField] private ColorToggle colorToggle1;
        [SerializeField] private ColorToggle colorToggle2;
        [SerializeField] private ColorToggle colorToggle3;

        [Header("Shape Carousels")]
        [SerializeField] private ShapeCarousel shapeCarousel1;
        [SerializeField] private ShapeCarousel shapeCarousel2;
        [SerializeField] private ShapeCarousel shapeCarousel3;

        [Header("Roguelike")]
        [SerializeField] private RoguelikeProgress roguelikeProgress;

=======
        [Header("Roguelike")]
        [SerializeField] private RoguelikeProgress roguelikeProgress;

        // Private fields
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
        private EnemyData currentEnemy;
        private EncodedSignal currentSignal;
        private int currentLevel = 1;
        private GeneratedMessage currentMessage;
        private bool isBattleInProgress = false;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
<<<<<<< HEAD
=======
            // Button subscriptions
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            if (sendButton != null) sendButton.onClick.AddListener(StartBattle);
            if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
            if (scoutButton != null) scoutButton.onClick.AddListener(ShowScoutInfo);
            if (restartButton != null) restartButton.onClick.AddListener(RestartGame);

<<<<<<< HEAD
=======
            // Initialization
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            currentSignal = new EncodedSignal();
            
            if (scoutPanel != null) scoutPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            
<<<<<<< HEAD
=======
            // Roguelike events subscription
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            if (roguelikeProgress != null)
            {
                roguelikeProgress.OnPlayerDeath += OnPlayerDeath;
                roguelikeProgress.OnRewardSelected += OnRewardSelected;
            }

<<<<<<< HEAD
=======
            // Start game
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            StartNewGame();
        }

        private void StartNewGame()
        {
            currentLevel = 1;
            isBattleInProgress = false;
            
            if (roguelikeProgress != null)
                roguelikeProgress.ResetProgress();
            
            GenerateNewLevel();
        }

        private void GenerateNewLevel()
        {
<<<<<<< HEAD
=======
            // Null check for UI components
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            if (messageText == null || levelCounterText == null)
            {
                Debug.LogError("[GenerateNewLevel] UI components not assigned in inspector!");
                return;
            }

            currentMessage = MessageGenerator.GenerateMessage(currentLevel);
            
            if (currentMessage == null)
            {
                Debug.LogError($"[GenerateNewLevel] Failed to generate message for level {currentLevel}");
                currentMessage = new GeneratedMessage 
                { 
                    messageText = "Attack north! No enemies at east. Hold position!",
                    requiredColors = new SignalColor[] { SignalColor.Red, SignalColor.Green, SignalColor.Red }
                };
            }
            
            messageText.text = $"LEVEL {currentLevel}: {currentMessage.messageText}";
            levelCounterText.text = $"LEVEL {currentLevel}";
            
            if (battleLogText != null)
                battleLogText.text = "Choose colors and shapes, then press SEND";
            
            if (nextLevelButton != null)
                nextLevelButton.interactable = false;
            
            if (sendButton != null)
                sendButton.interactable = true;
            
            isBattleInProgress = false;
<<<<<<< HEAD
=======
            
            Debug.Log($"[GenerateNewLevel] Level {currentLevel} created. HP: {roguelikeProgress?.CurrentHP}/{roguelikeProgress?.MaxHP}");
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
        }

        private void StartBattle()
        {
            if (isBattleInProgress)
            {
                Debug.LogWarning("[StartBattle] Battle already in progress!");
                return;
            }
            
            if (roguelikeProgress == null)
            {
                Debug.LogError("[StartBattle] RoguelikeProgress not assigned!");
                return;
            }
            
<<<<<<< HEAD
=======
            // Check and restore message
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            if (currentMessage == null)
            {
                Debug.LogWarning("[StartBattle] currentMessage = null, regenerating level!");
                GenerateNewLevel();
                
                if (currentMessage == null)
                {
                    battleLogText.text = "CRITICAL ERROR: Failed to create message!";
                    return;
                }
            }
            
            isBattleInProgress = true;
            
<<<<<<< HEAD
            currentSignal.colors[0] = colorToggle1.CurrentColor;
            currentSignal.colors[1] = colorToggle2.CurrentColor;
            currentSignal.colors[2] = colorToggle3.CurrentColor;

            currentSignal.shapes[0] = shapeCarousel1.CurrentShape;
            currentSignal.shapes[1] = shapeCarousel2.CurrentShape;
            currentSignal.shapes[2] = shapeCarousel3.CurrentShape;
            
            currentEnemy = GetCurrentEnemy();
            
            BattleResult result = BattleResolver.RunBattle(currentSignal, currentEnemy, out string log, roguelikeProgress);
            bool encryptionCorrect = IsMessageCorrectlyEncoded(currentSignal.colors, currentMessage);
            
=======
            // Collect signal from UI
            currentSignal.colors[0] = (SignalColor)color1Dropdown.value;
            currentSignal.colors[1] = (SignalColor)color2Dropdown.value;
            currentSignal.colors[2] = (SignalColor)color3Dropdown.value;
            currentSignal.shapes[0] = (ShapeType)shape1Dropdown.value;
            currentSignal.shapes[1] = (ShapeType)shape2Dropdown.value;
            currentSignal.shapes[2] = (ShapeType)shape3Dropdown.value;
            
            // Get enemy
            currentEnemy = GetCurrentEnemy();
            
            Debug.Log($"[StartBattle] Battle starting. Level {currentLevel}, Player HP: {roguelikeProgress.CurrentHP}/{roguelikeProgress.MaxHP}");
            
            // Run battle
            BattleResult result = BattleResolver.RunBattle(currentSignal, currentEnemy, out string log, roguelikeProgress);

            // Check encryption correctness
            bool encryptionCorrect = IsMessageCorrectlyEncoded(currentSignal.colors, currentMessage);

            // Form result
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            if (result == BattleResult.Victory)
            {
                if (encryptionCorrect)
                {
<<<<<<< HEAD
                    battleLogText.text = log + "\n\nVICTORY! Signal delivered. Ally UNDERSTOOD the message!";
=======
                    battleLogText.text = log + $"\n\nVICTORY! Signal delivered. Ally UNDERSTOOD the message!";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                    roguelikeProgress.ShowReward();
                    sendButton.interactable = false;
                }
                else
                {
<<<<<<< HEAD
=======
                    // GAME OVER - wrong encryption despite winning
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                    string requiredText = "";
                    for (int i = 0; i < 3; i++)
                    {
                        requiredText += $"Pos{i+1}: {(currentMessage.requiredColors[i] == SignalColor.Red ? "RED" : "GREEN")} ";
                    }
                    
<<<<<<< HEAD
                    battleLogText.text = log + "\n\nYOU WON THE BATTLE, BUT THE MESSAGE WAS WRONG!\n" +
                                        $"Required colors: {requiredText}\n" +
                                        "GAME OVER - Mission failed due to wrong encryption.";
=======
                    battleLogText.text = log + $"\n\nYOU WON THE BATTLE, BUT THE MESSAGE WAS WRONG!\n" +
                                        $"Required colors: {requiredText}\n" +
                                        $"GAME OVER - Mission failed due to wrong encryption.";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                    roguelikeProgress.ForceKill();
                    sendButton.interactable = false;
                    nextLevelButton.interactable = false;
                }
            }
            else
            {
<<<<<<< HEAD
                battleLogText.text = log + "\n\nDEFEAT! Signal intercepted. Game Over.";
=======
                // GAME OVER - lost the battle
                battleLogText.text = log + $"\n\nDEFEAT! Signal intercepted. Game Over.";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                sendButton.interactable = false;
                nextLevelButton.interactable = false;
            }
        }

        private void NextLevel()
        {
            if (isBattleInProgress)
            {
                Debug.LogWarning("[NextLevel] Cannot go to next level during battle!");
                return;
            }
            
            currentLevel++;
            GenerateNewLevel();
<<<<<<< HEAD
=======
            Debug.Log($"[NextLevel] Moving to level {currentLevel}");
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
        }

        private void OnRewardSelected()
        {
            if (nextLevelButton != null)
                nextLevelButton.interactable = true;
            if (sendButton != null)
                sendButton.interactable = false;
            
            isBattleInProgress = false;
<<<<<<< HEAD
=======
            Debug.Log("[OnRewardSelected] Reward chosen, can proceed to next level");
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                if (gameOverScoreText != null)
                    gameOverScoreText.text = $"Levels completed: {currentLevel - 1}";
                
                gameOverPanel.SetActive(true);
            }
            
            if (sendButton != null) sendButton.interactable = false;
            if (nextLevelButton != null) nextLevelButton.interactable = false;
            if (scoutButton != null) scoutButton.interactable = false;
            
            isBattleInProgress = false;
        }

        private void RestartGame()
        {
<<<<<<< HEAD
=======
            Debug.Log("[RestartGame] Restarting game");
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            StartNewGame();
            
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            
            if (battleLogText != null)
                battleLogText.text = "New game! Choose colors and shapes.";
            
            if (scoutButton != null) scoutButton.interactable = true;
        }

        private void OnPlayerDeath()
        {
            ShowGameOver();
        }

        private void ShowScoutInfo()
        {
            if (currentEnemy == null)
            {
<<<<<<< HEAD
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = "???";
                if (scoutShapesText != null) scoutShapesText.text = "Start battle first";
=======
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = "Enemy unknown";
                if (scoutShapesText != null) scoutShapesText.text = "Start a battle to see the enemy";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                if (scoutTurnOrderText != null) scoutTurnOrderText.text = "";
            }
            else
            {
<<<<<<< HEAD
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = currentEnemy.enemyName;
=======
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = $"ENEMY: {currentEnemy.enemyName}";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                
                string shapesString = "";
                foreach (var shape in currentEnemy.shapes)
                {
                    shapesString += GetShapeSymbol(shape) + " -> ";
                }
                shapesString = shapesString.TrimEnd(' ', '-', '>');
                
<<<<<<< HEAD
                if (scoutShapesText != null) scoutShapesText.text = shapesString;
                
                if (scoutTurnOrderText != null)
                {
                    scoutTurnOrderText.text = currentEnemy.playerFirst ? "ENEMY FIRST" : "YOU FIRST";
=======
                if (scoutShapesText != null) scoutShapesText.text = $"CHAIN: {shapesString}";
                
                if (scoutTurnOrderText != null)
                {
                    scoutTurnOrderText.text = currentEnemy.playerFirst ? 
                        "FIRST MOVE: ENEMY" : 
                        "FIRST MOVE: YOU";
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
                }
            }
            
            if (scoutPanel != null)
                scoutPanel.SetActive(true);
        }

        public void CloseScoutPanel()
        {
            if (scoutPanel != null)
                scoutPanel.SetActive(false);
        }

        private string GetShapeSymbol(ShapeType shape)
        {
            switch (shape)
            {
                case ShapeType.Square: return "Square";
                case ShapeType.Triangle: return "Triangle";
                case ShapeType.Circle: return "Circle";
                default: return "?";
            }
        }

        private EnemyData GetCurrentEnemy()
        {
            int baseHp = 30 + (currentLevel - 1) * 5;
            int baseAttack = 10 + (currentLevel - 1) * 2;
            
            ShapeType[][] possiblePatterns = new ShapeType[][]
            {
                new ShapeType[] { ShapeType.Triangle, ShapeType.Square, ShapeType.Circle },
                new ShapeType[] { ShapeType.Square, ShapeType.Triangle, ShapeType.Square },
                new ShapeType[] { ShapeType.Circle, ShapeType.Triangle, ShapeType.Triangle },
                new ShapeType[] { ShapeType.Square, ShapeType.Circle, ShapeType.Square },
                new ShapeType[] { ShapeType.Triangle, ShapeType.Circle, ShapeType.Triangle },
                new ShapeType[] { ShapeType.Circle, ShapeType.Square, ShapeType.Triangle }
            };
            
            var pattern = possiblePatterns[Random.Range(0, possiblePatterns.Length)];
            bool playerFirst = Random.Range(0, 100) > (15 + currentLevel * 2);
            
            string[] enemyNames = { "Scanner", "Jammer", "Interceptor", "Encryptor", "Analyzer", "Kraken", "Ghost", "Silence" };
            string name = enemyNames[Random.Range(0, enemyNames.Length)] + (currentLevel > 10 ? "+" : "");
            
            return new EnemyData(
                name,
                pattern,
                playerFirst,
                baseHp,
                baseAttack
            );
        }

        private bool IsMessageCorrectlyEncoded(SignalColor[] colors, GeneratedMessage message)
        {
            if (message == null || message.requiredColors == null) return false;
            
<<<<<<< HEAD
=======
            // Проверяем каждую позицию
>>>>>>> 85ecedfe888998b7f87a2790d2757d5ab07d70ff
            for (int i = 0; i < 3; i++)
            {
                if (colors[i] != message.requiredColors[i]) return false;
            }
            
            return true;
        }
    }
}