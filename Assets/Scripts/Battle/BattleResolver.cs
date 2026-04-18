// LUMI :)
using UnityEngine;

namespace SignalRoguelite
{
    public class BattleResolver
    {
        // Результат одного шага
        public struct StepResult
        {
            public int playerDamage;      // урон игрока по врагу
            public int playerHeal;        // лечение игрока
            public int enemyDamage;       // урон врага по игроку
            public bool blockedEnemy;     // заблокирована ли атака врага
            public string logMessage;     // текстовое описание шага
        }

        // Обработка одного шага
        public static StepResult ResolveStep(ShapeType playerShape, ShapeType enemyShape, int enemyAttackPower)
        {
            StepResult result = new StepResult();
            result.playerDamage = 0;
            result.playerHeal = 0;
            result.enemyDamage = 0;
            result.blockedEnemy = false;

            // --- Действие игрока ---
            switch (playerShape)
            {
                case ShapeType.Triangle:  // Атака
                    result.playerDamage = 10;
                    result.logMessage = "Ты атакуешь!";
                    break;
                    
                case ShapeType.Circle:    // Лечение
                    result.playerHeal = 8;
                    result.logMessage = "Ты лечишься!";
                    break;
                    
                case ShapeType.Square:    // Парирование
                    result.logMessage = "Ты готовишься парировать...";
                    break;
            }

            // --- Действие врага (если не заблокирован) ---
            bool enemyBlocked = (playerShape == ShapeType.Square && enemyShape == ShapeType.Triangle);
            
            if (!enemyBlocked)
            {
                switch (enemyShape)
                {
                    case ShapeType.Triangle:
                        result.enemyDamage = enemyAttackPower;
                        result.logMessage += " Враг атакует!";
                        break;
                    case ShapeType.Circle:
                        // Враг лечится (не влияет на игрока)
                        result.logMessage += " Враг лечится.";
                        break;
                    case ShapeType.Square:
                        // Враг парирует (не наносит урон)
                        result.logMessage += " Враг парирует.";
                        break;
                }
            }
            else
            {
                result.logMessage += " Ты парировал атаку врага!";
                result.blockedEnemy = true;
            }

            return result;
        }

        // Полная битва (3 шага)
        public static BattleResult RunBattle(EncodedSignal playerSignal, EnemyData enemy, out string battleLog, RoguelikeProgress roguelikeProgress)
        {
            int playerHp = roguelikeProgress != null ? roguelikeProgress.currentHP : 100;
            int enemyHp = enemy.enemyHp;
            battleLog = "=== БИТВА НАЧАЛАСЬ ===\n";

            for (int step = 0; step < 3; step++)
            {
                battleLog += $"\n--- Шаг {step + 1} ---\n";
                
                // Кто ходит первым?
                if (enemy.playerFirst)
                {
                    // Враг ходит первым (сложный режим)
                    battleLog += "Враг атакует первым!\n";
                    // Сначала обрабатываем атаку врага (упрощённо)
                    if (playerSignal.shapes[step] != ShapeType.Square)
                    {
                        playerHp -= enemy.attackPower;
                        battleLog += $"Враг нанёс {enemy.attackPower} урона! HP: {playerHp}\n";
                    }
                    else
                    {
                        battleLog += "Ты парировал! Урона нет.\n";
                    }
                    
                    // Потом действие игрока
                    StepResult stepResult = ResolveStep(playerSignal.shapes[step], enemy.shapes[step], enemy.attackPower);
                    ApplyStepResult(ref playerHp, ref enemyHp, stepResult);
                    battleLog += stepResult.logMessage + $"\nHP игрока: {playerHp} | HP врага: {enemyHp}\n";
                }
                else
                {
                    // Игрок ходит первым
                    StepResult stepResult = ResolveStep(playerSignal.shapes[step], enemy.shapes[step], enemy.attackPower);
                    ApplyStepResult(ref playerHp, ref enemyHp, stepResult);
                    battleLog += stepResult.logMessage + $"\nHP игрока: {playerHp} | HP врага: {enemyHp}\n";
                }

                // Проверка смерти
                if (playerHp <= 0)
                {
                    battleLog += "\n❌ ТВОЙ СИГНАЛ ПЕРЕХВАЧЕН! Линия связи оборвана.\n";
                    return BattleResult.Defeat;
                }
                
                if (enemyHp <= 0)
                {
                    battleLog += "\n✅ ВРАГ ПОВЕРЖЕН! Сигнал доставлен!\n";
                    return BattleResult.Victory;
                }
            }

            // После битвы обновляем HP игрока
            if (roguelikeProgress != null)
            {
                roguelikeProgress.currentHP = playerHp;
                roguelikeProgress.UpdateUI();
            }

            // После 3 шагов проверяем, кто выжил
            if (playerHp > 0 && enemyHp > 0)
            {
                battleLog += "\n⚖️ Битва окончена, но сигнал всё ещё в пути...";
                return BattleResult.Victory;
            }
            
            return (playerHp > 0) ? BattleResult.Victory : BattleResult.Defeat;
        }

        private static void ApplyStepResult(ref int playerHp, ref int enemyHp, StepResult result)
        {
            playerHp += result.playerHeal;
            playerHp -= result.enemyDamage;
            enemyHp -= result.playerDamage;
            
            // Ограничения
            playerHp = Mathf.Max(0, playerHp);
            enemyHp = Mathf.Max(0, enemyHp);
        }
    }
}