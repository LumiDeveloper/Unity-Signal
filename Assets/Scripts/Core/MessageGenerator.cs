using System;
using System.Collections.Generic;

namespace SignalRoguelite
{
    public static class MessageGenerator
    {
        private static Random random = new Random();
        
        // Фразы для атаки (требуют КРАСНЫЙ цвет)
        private static string[] attackPhrases = new string[]
        {
            "Attack", "Strike", "Charge", "Assault", "Hit", "Push", "Storm"
        };
        
        // Фразы для "нет врагов" (требуют ЗЕЛЁНЫЙ цвет)
        private static string[] noEnemyPhrases = new string[]
        {
            "No enemies", "Clear", "Empty", "Safe", "Quiet", "All clear"
        };
        
        // Фразы для нейтральных/общих действий (могут быть любым цветом)
        private static string[] neutralPhrases = new string[]
        {
            "Hold position", "Stay alert", "Wait", "Observe", "Report"
        };
        
        private static string[] locations = new string[]
        {
            "north", "south", "east", "west", "center", "frontline", "rear", "left flank", "right flank"
        };
        
        private static string[] urgency = new string[]
        {
            "URGENT!", "IMMEDIATELY!", "FAST!", "ATTENTION!", "IMPORTANT!"
        };
        
        /// <summary>
        /// Generates a message with 3 parts (start, middle, end)
        /// Each part requires a specific color (Red or Green)
        /// </summary>
        public static GeneratedMessage GenerateMessage(int level)
        {
            // Массив для 3 цветов (начало, середина, конец)
            SignalColor[] requiredColors = new SignalColor[3];
            string[] messageParts = new string[3];
            
            // Определяем тип сообщения на основе уровня
            int messageType = DetermineMessageType(level);
            
            switch (messageType)
            {
                case 0: // Простое: атака → атака → атака
                    requiredColors[0] = SignalColor.Red;
                    requiredColors[1] = SignalColor.Red;
                    requiredColors[2] = SignalColor.Red;
                    
                    messageParts[0] = GetAttackPart();
                    messageParts[1] = GetAttackPart();
                    messageParts[2] = GetAttackPart();
                    break;
                    
                case 1: // Простое: нет врагов → нет врагов → нет врагов
                    requiredColors[0] = SignalColor.Green;
                    requiredColors[1] = SignalColor.Green;
                    requiredColors[2] = SignalColor.Green;
                    
                    messageParts[0] = GetNoEnemyPart();
                    messageParts[1] = GetNoEnemyPart();
                    messageParts[2] = GetNoEnemyPart();
                    break;
                    
                case 2: // Смешанное: атака → нет врагов → атака
                    requiredColors[0] = SignalColor.Red;
                    requiredColors[1] = SignalColor.Green;
                    requiredColors[2] = SignalColor.Red;
                    
                    messageParts[0] = GetAttackPart();
                    messageParts[1] = GetNoEnemyPart();
                    messageParts[2] = GetAttackPart();
                    break;
                    
                case 3: // Смешанное: нет врагов → атака → нет врагов
                    requiredColors[0] = SignalColor.Green;
                    requiredColors[1] = SignalColor.Red;
                    requiredColors[2] = SignalColor.Green;
                    
                    messageParts[0] = GetNoEnemyPart();
                    messageParts[1] = GetAttackPart();
                    messageParts[2] = GetNoEnemyPart();
                    break;
                    
                case 4: // Сложное: атака → нейтральное → нет врагов
                    requiredColors[0] = SignalColor.Red;
                    requiredColors[1] = GetRandomColor();
                    requiredColors[2] = SignalColor.Green;
                    
                    messageParts[0] = GetAttackPart();
                    messageParts[1] = GetNeutralPart();
                    messageParts[2] = GetNoEnemyPart();
                    break;
                    
                case 5: // Сложное: нет врагов → нейтральное → атака
                    requiredColors[0] = SignalColor.Green;
                    requiredColors[1] = GetRandomColor();
                    requiredColors[2] = SignalColor.Red;
                    
                    messageParts[0] = GetNoEnemyPart();
                    messageParts[1] = GetNeutralPart();
                    messageParts[2] = GetAttackPart();
                    break;
                    
                default: // Случайное
                    for (int i = 0; i < 3; i++)
                    {
                        requiredColors[i] = GetRandomColor();
                        messageParts[i] = GetPartByColor(requiredColors[i]);
                    }
                    break;
            }
            
            // Собираем сообщение
            string fullMessage = $"{messageParts[0]} {messageParts[1]} {messageParts[2]}";
            
            // Добавляем срочность на высоких уровнях
            if (level > 5 && random.Next(3) == 0)
            {
                fullMessage = urgency[random.Next(urgency.Length)] + " " + fullMessage;
            }
            
            return new GeneratedMessage
            {
                messageText = fullMessage,
                requiredColors = requiredColors
            };
        }
        
        private static int DetermineMessageType(int level)
        {
            if (level <= 3)
            {
                // Уровни 1-3: простые сообщения (0-1)
                return random.Next(2);
            }
            else if (level <= 7)
            {
                // Уровни 4-7: смешанные (2-3)
                return random.Next(2, 4);
            }
            else
            {
                // Уровни 8+: сложные (4-6)
                return random.Next(4, 7);
            }
        }
        
        private static SignalColor GetRandomColor()
        {
            return random.Next(2) == 0 ? SignalColor.Red : SignalColor.Green;
        }
        
        private static string GetPartByColor(SignalColor color)
        {
            return color == SignalColor.Red ? GetAttackPart() : GetNoEnemyPart();
        }
        
        private static string GetAttackPart()
        {
            string phrase = attackPhrases[random.Next(attackPhrases.Length)];
            string loc = locations[random.Next(locations.Length)];
            return $"{phrase} {loc}!";
        }
        
        private static string GetNoEnemyPart()
        {
            string phrase = noEnemyPhrases[random.Next(noEnemyPhrases.Length)];
            string loc = locations[random.Next(locations.Length)];
            return $"{phrase} at {loc}.";
        }
        
        private static string GetNeutralPart()
        {
            string phrase = neutralPhrases[random.Next(neutralPhrases.Length)];
            string loc = locations[random.Next(locations.Length)];
            return $"{phrase} at {loc}.";
        }
        
        private static string FirstUpper(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
    
    public class GeneratedMessage
    {
        public string messageText;
        public SignalColor[] requiredColors; // 3 цвета: для начала, середины, конца
    }
}