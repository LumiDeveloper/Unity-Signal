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
        public TMP_Dropdown color1Dropdown;
        public TMP_Dropdown color2Dropdown;
        public TMP_Dropdown color3Dropdown;
        public TMP_Dropdown shape1Dropdown;
        public TMP_Dropdown shape2Dropdown;
        public TMP_Dropdown shape3Dropdown;     

        public TMP_Text battleLogText;          // Кнопка отправки сообщения
        public TMP_Text messageText;            // Текст сообщения
        public Button sendButton;               // Кнопка отправки сообщения
        public Button nextLevelButton;          // Кнопка следующего уровня

        [Header("Scout")]
        public GameObject scoutPanel;           // Ссылка на окно разведки
        public TMP_Text scoutEnemyNameText;     // Имя врага
        public TMP_Text scoutShapesText;        // Цепочка фигур врага
        public TMP_Text scoutTurnOrderText;     // Очерёдность
        public Button scoutButton;              // Кнопка "Разведка"

        [Header("Level Counter")]
        public TMP_Text levelCounterText;       // Счетчик уровней


        // Внешние скрипты (Roguelike)
        [Header("Roguelike")]
        public RoguelikeProgress roguelikeProgress;


        
        // Переменные кодирования и уровни
        private EnemyData currentEnemy;
        private EncodedSignal currentSignal;
        private int currentLevel = 1;

        // Список сообщений и правильных ответов
        private MessageLevel[] levels = new MessageLevel[]
        {
            new MessageLevel("На северном посту врагов нет. Атакуй южный фронт!", true, true),
            new MessageLevel("Врагов нет! Всем стоять на месте.", true, false),
            new MessageLevel("Атакуй! Атакуй! Никого не щади!", false, true),
            new MessageLevel("Обстановка спокойная. Врагов нет. Отдыхайте.", true, false),
            new MessageLevel("Врагов нет на востоке. Атакуй западный фланг!", true, true)
        };
        










// ================================= Функциональность игры, в основном здесь лежит вообще всё, будь моя воля переписал на модульную архитектуру ====================================================


        void Start()
        {
            sendButton.onClick.AddListener(StartBattle);
            nextLevelButton.onClick.AddListener(NextLevel);
            nextLevelButton.interactable = false;  // Сначала скрыта/неактивна
            currentSignal = new EncodedSignal();
            scoutButton.onClick.AddListener(ShowScoutInfo);
            scoutPanel.SetActive(false);

            if (roguelikeProgress != null)
            {
                roguelikeProgress.OnPlayerDeath += OnPlayerDeath;
            }

            if (roguelikeProgress != null)
            {
                roguelikeProgress.OnRewardSelected += OnRewardSelected;
            }
            
            LoadLevel(0);
        }

        void OnRewardSelected()
        {
            // Переход на следующий уровень после выбора награды
            nextLevelButton.interactable = true;
            sendButton.interactable = false;
        }


// ------------------------------------- Работа со сценами уровня -------------------------------------------------------------
        
        void LoadLevel(int levelIndex)
        {
            // Защита от null
            if (messageText == null)
            {
                Debug.LogError("messageText не назначен в инспекторе!");
                return;
            }
            
            if (levelCounterText == null)
            {
                Debug.LogError("levelCounterText не назначен в инспекторе!");
                return;
            }
            
            if (levels == null || levels.Length == 0)
            {
                Debug.LogError("Массив levels пуст или не назначен!");
                return;
            }
            
            if (levelIndex >= levels.Length)
            {
                if (messageText != null)
                    messageText.text = "🎉 ПОЗДРАВЛЯЮ! Ты прошёл всю игру! 🎉";
                if (battleLogText != null)
                    battleLogText.text = "Спасибо за игру!";
                if (sendButton != null)
                    sendButton.interactable = false;
                if (nextLevelButton != null)
                    nextLevelButton.interactable = false;
                return;
            }
            
            var level = levels[levelIndex];
            messageText.text = $"📨 УРОВЕНЬ {currentLevel}: {level.message}";
            levelCounterText.text = $"📊 УРОВЕНЬ {currentLevel} / {levels.Length}";
            
            if (battleLogText != null)
                battleLogText.text = "Выбери цвета и фигуры, затем нажми ОТПРАВИТЬ";
            
            if (nextLevelButton != null)
                nextLevelButton.interactable = false;
            
            if (sendButton != null)
                sendButton.interactable = true;
        }

        void NextLevel()
        {
            currentLevel++;
            LoadLevel(currentLevel - 1);
        }














// ------------------------------------- Работа с врагами и сражениями в игре -------------------------------------------------------------

        void StartBattle()
        {
            // Собираем сигнал из UI
            currentSignal.colors[0] = (SignalColor)color1Dropdown.value;
            currentSignal.colors[1] = (SignalColor)color2Dropdown.value;
            currentSignal.colors[2] = (SignalColor)color3Dropdown.value;
            currentSignal.shapes[0] = (ShapeType)shape1Dropdown.value;
            currentSignal.shapes[1] = (ShapeType)shape2Dropdown.value;
            currentSignal.shapes[2] = (ShapeType)shape3Dropdown.value;
            
            // ПОЛУЧАЕМ ТЕКУЩЕГО ВРАГА
            currentEnemy = GetCurrentEnemy();
            
            // Бонусы от рогалика
            int attackBonus = roguelikeProgress != null ? roguelikeProgress.attackBonus : 0;
            int healBonus = roguelikeProgress != null ? roguelikeProgress.healBonus : 0;
            
            // ЗАПУСКАЕМ БИТВУ (только один раз!)
            BattleResult result = BattleResolver.RunBattle(currentSignal, currentEnemy, out string log, roguelikeProgress);
            
            // Данные текущего уровня
            var currentLevelData = levels[currentLevel - 1];
            bool encryptionCorrect = IsMessageCorrectlyEncoded(currentSignal.colors, currentLevelData);
            
            // Формируем результат
            if (result == BattleResult.Victory)
            {
                if (encryptionCorrect)
                {
                    battleLogText.text = log + $"\n\n✅ ПОБЕДА! Сигнал доставлен. Союзник ПОНЯЛ сообщение!";
                    
                    if (roguelikeProgress != null)
                        roguelikeProgress.ShowReward();
                    else
                        nextLevelButton.interactable = true;
                        
                    sendButton.interactable = false;
                }
                else
                {
                    battleLogText.text = log + $"\n\n⚠️ Ты победил в бою, но НЕПРАВИЛЬНО зашифровал смысл!\n" +
                                        $"Союзник не понял: {(NeedRed(currentLevelData) ? "нужен был КРАСНЫЙ" : "")} " +
                                        $"{(NeedGreen(currentLevelData) ? "нужен был ЗЕЛЁНЫЙ" : "")}\n" +
                                        $"Попробуй ещё раз на этом уровне.";
                    nextLevelButton.interactable = false;
                    sendButton.interactable = true;
                }
            }
            else
            {
                battleLogText.text = log + $"\n\n❌ ПОРАЖЕНИЕ! Сигнал перехвачен. Попробуй ещё раз.";
                nextLevelButton.interactable = false;
                sendButton.interactable = true;
            }
        }

        // Массив врагов для каждого уровня
        private EnemyData[] enemies = new EnemyData[]
        {
            new EnemyData("Сканер", new ShapeType[] { ShapeType.Triangle, ShapeType.Square, ShapeType.Circle }, false, 20, 8),
            new EnemyData("Глушилка", new ShapeType[] { ShapeType.Square, ShapeType.Triangle, ShapeType.Square }, true, 25, 10),
            new EnemyData("Перехватчик", new ShapeType[] { ShapeType.Circle, ShapeType.Triangle, ShapeType.Triangle }, false, 30, 12),
            new EnemyData("Шифровальщик", new ShapeType[] { ShapeType.Square, ShapeType.Circle, ShapeType.Square }, true, 35, 14),
            new EnemyData("Анализатор", new ShapeType[] { ShapeType.Triangle, ShapeType.Circle, ShapeType.Triangle }, false, 40, 15),
        };

        void OnPlayerDeath()
        {
            battleLogText.text = "💀 ТВОЙ ОПЕРАТОР ПОГИБ! Игра окончена. 💀\nНажмите Restart в меню.";
            sendButton.interactable = false;
            nextLevelButton.interactable = false;
        }




// ------------------------------------- Работа с цветами и фигурами в игре, шифрование и определение цветов -------------------------------------------------------------

        
        // Проверка: нужен ли в сообщении красный (Атакуй)
        private bool NeedRed(MessageLevel level)
        {
            return level.needAttack;
        }
        
        // Проверка: нужен ли в сообщении зелёный (Врагов нет)
        private bool NeedGreen(MessageLevel level)
        {
            return level.needNoEnemies;
        }
        
        // Проверка правильности шифрования
        private bool IsMessageCorrectlyEncoded(SignalColor[] colors, MessageLevel level)
        {
            bool hasRed = colors.Contains(SignalColor.Red);
            bool hasGreen = colors.Contains(SignalColor.Green);
            
            if (NeedRed(level) && !hasRed) return false;
            if (NeedGreen(level) && !hasGreen) return false;
            
            return true;
        }

        void ShowScoutInfo()
        {
            var currentEnemy = GetCurrentEnemy();
            
            scoutEnemyNameText.text = $"🕵️ ВРАГ: {currentEnemy.enemyName}";
            
            // Преобразуем фигуры в текст
            string shapesString = "";
            foreach (var shape in currentEnemy.shapes)
            {
                shapesString += GetShapeSymbol(shape) + " → ";
            }
            shapesString = shapesString.TrimEnd(' ', '→');
            scoutShapesText.text = $"🎴 ЦЕПОЧКА: {shapesString}";
            
            scoutTurnOrderText.text = currentEnemy.playerFirst ? 
                "⚔️ ПЕРВЫМ ХОДИТ: ВРАГ" : 
                "⚔️ ПЕРВЫМ ХОДИШЬ: ТЫ";
            
            scoutPanel.SetActive(true);
        }


        // Структура для проверки позиции цвета
        public class ColorPositionRequirement
        {
            public SignalColor color;
            public ColorPosition position;  // Start, Middle, End
            public string requiredWord;     // слово, которое должно быть в сообщении
            
            public ColorPositionRequirement(SignalColor c, ColorPosition pos, string word)
            {
                color = c;
                position = pos;
                requiredWord = word;
            }
        }

        // Массив требований для проверки сообщений
        private ColorPositionRequirement[] colorRules = new ColorPositionRequirement[]
        {
            new ColorPositionRequirement(SignalColor.Red, ColorPosition.Start, "атакуй"),
            new ColorPositionRequirement(SignalColor.Red, ColorPosition.Middle, "много"),
            new ColorPositionRequirement(SignalColor.Red, ColorPosition.End, "закат"),
            new ColorPositionRequirement(SignalColor.Green, ColorPosition.Start, "защищайся"),
            new ColorPositionRequirement(SignalColor.Green, ColorPosition.Middle, "нет"),
            new ColorPositionRequirement(SignalColor.Green, ColorPosition.End, "поле"),
        };

        // Новая проверка шифрования с учётом позиции
        bool IsMessageCorrectlyEncodedWithPosition(SignalColor[] colors, string message)
        {
            string lowerMsg = message.ToLower();
            
            for (int i = 0; i < 3; i++)
            {
                SignalColor color = colors[i];
                ColorPosition position = (ColorPosition)i;  // 0=Start, 1=Middle, 2=End
                
                // Находим правило для этой комбинации
                var rule = colorRules.FirstOrDefault(r => r.color == color && r.position == position);
                
                if (rule != null)
                {

                    if (lowerMsg.Contains(rule.requiredWord))
                    {

                    }
                    else
                    {
                        bool requiredByOtherRule = colorRules.Any(r => 
                            r.position == position && lowerMsg.Contains(r.requiredWord));
                        
                        if (!requiredByOtherRule)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }
        






// ----------------------------------------------------- Вспомогательные классы и методы обработка (враги, уровни) -------------------------------------------------------------

        string GetShapeSymbol(ShapeType shape)
        {
            switch (shape)
            {
                case ShapeType.Square: return "⬛ Квадрат";
                case ShapeType.Triangle: return "▲ Треугольник";
                case ShapeType.Circle: return "● Круг";
                default: return "?";
            }
        }

        EnemyData GetCurrentEnemy()
        {
            int index = (currentLevel - 1) % enemies.Length;
            var enemy = enemies[index];
            
            int extraHp = (currentLevel - 1) * 3;
            int extraAttack = (currentLevel - 1) * 1;
            
            return new EnemyData(
                enemy.enemyName + (currentLevel > enemies.Length ? "+" : ""),
                enemy.shapes,
                enemy.playerFirst,
                enemy.enemyHp + extraHp,
                enemy.attackPower + extraAttack
            );
        }

        // Добавь метод для закрытия окна (повесь на кнопку "Закрыть")
        public void CloseScoutPanel()
        {
            scoutPanel.SetActive(false);
        }

        // Класс для хранения уровня
        [System.Serializable]
        public class MessageLevel
        {
            public string message;
            public bool needNoEnemies;
            public bool needAttack;
            
            public MessageLevel(string msg, bool needNoEnemies, bool needAttack)
            {
                this.message = msg;
                this.needNoEnemies = needNoEnemies;
                this.needAttack = needAttack;
            }
        }
    }
}