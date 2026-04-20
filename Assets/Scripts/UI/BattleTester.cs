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
        [SerializeField] private TMP_Text battleLogText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Button nextLevelButton;

        [Header("Color Selectors")]
        [SerializeField] private ColorToggle colorToggle1;
        [SerializeField] private ColorToggle colorToggle2;
        [SerializeField] private ColorToggle colorToggle3;

        [Header("Shape Carousels")]
        [SerializeField] private ShapeCarousel shapeCarousel1;
        [SerializeField] private ShapeCarousel shapeCarousel2;
        [SerializeField] private ShapeCarousel shapeCarousel3;

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

        [Header("Roguelike")]
        [SerializeField] private RoguelikeProgress roguelikeProgress;

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
            if (sendButton != null) sendButton.onClick.AddListener(StartBattle);
            if (nextLevelButton != null) nextLevelButton.onClick.AddListener(NextLevel);
            if (scoutButton != null) scoutButton.onClick.AddListener(ShowScoutInfo);
            if (restartButton != null) restartButton.onClick.AddListener(RestartGame);

            currentSignal = new EncodedSignal();
            
            if (scoutPanel != null) scoutPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            
            if (roguelikeProgress != null)
            {
                roguelikeProgress.OnPlayerDeath += OnPlayerDeath;
                roguelikeProgress.OnRewardSelected += OnRewardSelected;
            }

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
            
            // Collect signal from ColorToggles and ShapeCarousels
            currentSignal.colors[0] = colorToggle1.CurrentColor;
            currentSignal.colors[1] = colorToggle2.CurrentColor;
            currentSignal.colors[2] = colorToggle3.CurrentColor;

            currentSignal.shapes[0] = shapeCarousel1.CurrentShape;
            currentSignal.shapes[1] = shapeCarousel2.CurrentShape;
            currentSignal.shapes[2] = shapeCarousel3.CurrentShape;
            
            currentEnemy = GetCurrentEnemy();
            
            BattleResult result = BattleResolver.RunBattle(currentSignal, currentEnemy, out string log, roguelikeProgress);
            bool encryptionCorrect = IsMessageCorrectlyEncoded(currentSignal.colors, currentMessage);
            
            if (result == BattleResult.Victory)
            {
                if (encryptionCorrect)
                {
                    battleLogText.text = log + "\n\nVICTORY! Signal delivered. Ally UNDERSTOOD the message!";
                    roguelikeProgress.ShowReward();
                    sendButton.interactable = false;
                }
                else
                {
                    string requiredText = "";
                    for (int i = 0; i < 3; i++)
                    {
                        requiredText += $"Pos{i+1}: {(currentMessage.requiredColors[i] == SignalColor.Red ? "RED" : "GREEN")} ";
                    }
                    
                    battleLogText.text = log + "\n\nYOU WON THE BATTLE, BUT THE MESSAGE WAS WRONG!\n" +
                                        $"Required colors: {requiredText}\n" +
                                        "GAME OVER - Mission failed due to wrong encryption.";
                    roguelikeProgress.ForceKill();
                    sendButton.interactable = false;
                    nextLevelButton.interactable = false;
                }
            }
            else
            {
                battleLogText.text = log + "\n\nDEFEAT! Signal intercepted. Game Over.";
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
        }

        private void OnRewardSelected()
        {
            if (nextLevelButton != null)
                nextLevelButton.interactable = true;
            if (sendButton != null)
                sendButton.interactable = false;
            
            isBattleInProgress = false;
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
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = "???";
                if (scoutShapesText != null) scoutShapesText.text = "Start battle first";
                if (scoutTurnOrderText != null) scoutTurnOrderText.text = "";
            }
            else
            {
                if (scoutEnemyNameText != null) scoutEnemyNameText.text = currentEnemy.enemyName;
                
                string shapesString = "";
                foreach (var shape in currentEnemy.shapes)
                {
                    shapesString += GetShapeSymbol(shape) + " -> ";
                }
                shapesString = shapesString.TrimEnd(' ', '-', '>');
                
                if (scoutShapesText != null) scoutShapesText.text = shapesString;
                
                if (scoutTurnOrderText != null)
                {
                    scoutTurnOrderText.text = currentEnemy.playerFirst ? "ENEMY FIRST" : "YOU FIRST";
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
            
            for (int i = 0; i < 3; i++)
            {
                if (colors[i] != message.requiredColors[i]) return false;
            }
            
            return true;
        }
    }
}