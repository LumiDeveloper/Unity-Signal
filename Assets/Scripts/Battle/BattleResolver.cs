// LUMI :)
using UnityEngine;

namespace SignalRoguelite
{
    public class BattleResolver
    {
        // Result of one step
        public struct StepResult
        {
            public int playerDamage;
            public int playerHeal;
            public int enemyDamage;
            public bool blockedEnemy;
            public string logMessage;
        }

        // Process one step
        public static StepResult ResolveStep(ShapeType playerShape, ShapeType enemyShape, 
            int enemyAttackPower, int playerAttackBonus = 0, int playerHealBonus = 0)
        {
            StepResult result = new StepResult
            {
                playerDamage = 0,
                playerHeal = 0,
                enemyDamage = 0,
                blockedEnemy = false,
                logMessage = ""
            };

            // --- Player action with bonuses ---
            switch (playerShape)
            {
                case ShapeType.Triangle:
                    result.playerDamage = 10 + playerAttackBonus;
                    result.logMessage = $"You attack! (+{playerAttackBonus})";
                    break;
                    
                case ShapeType.Circle:
                    result.playerHeal = 8 + playerHealBonus;
                    result.logMessage = $"You heal! (+{playerHealBonus})";
                    break;
                    
                case ShapeType.Square:
                    result.logMessage = "You prepare to parry...";
                    break;
            }

            // --- Enemy action ---
            bool enemyBlocked = (playerShape == ShapeType.Square && enemyShape == ShapeType.Triangle);
            
            if (!enemyBlocked && enemyShape == ShapeType.Triangle)
            {
                result.enemyDamage = enemyAttackPower;
                result.logMessage += " Enemy attacks!";
            }
            else if (enemyBlocked)
            {
                result.logMessage += " You parried the enemy's attack!";
                result.blockedEnemy = true;
            }
            else if (enemyShape == ShapeType.Circle)
            {
                result.logMessage += " Enemy heals.";
            }
            else if (enemyShape == ShapeType.Square)
            {
                result.logMessage += " Enemy parries.";
            }

            return result;
        }

        // Full battle (3 steps)
        public static BattleResult RunBattle(EncodedSignal playerSignal, EnemyData enemy, 
            out string battleLog, RoguelikeProgress roguelikeProgress)
        {
            battleLog = "=== BATTLE STARTED ===\n";
            
            if (roguelikeProgress == null)
            {
                battleLog += "\nERROR: RoguelikeProgress not assigned!\n";
                return BattleResult.Defeat;
            }
            
            // Get player stats via properties
            int playerHp = roguelikeProgress.CurrentHP;
            int attackBonus = roguelikeProgress.AttackBonus;
            int healBonus = roguelikeProgress.HealBonus;
            int enemyHp = enemy.enemyHp;
            
            Debug.Log($"[RunBattle] Battle start. PlayerHP: {playerHp}/{roguelikeProgress.MaxHP}, " +
                      $"AttackBonus: +{attackBonus}, HealBonus: +{healBonus}");
            Debug.Log($"[RunBattle] Enemy: {enemy.enemyName}, HP: {enemyHp}, Attack: {enemy.attackPower}, PlayerFirst: {enemy.playerFirst}");
            
            for (int step = 0; step < 3; step++)
            {
                StepResult stepResult = ResolveStep(playerSignal.shapes[step], enemy.shapes[step], 
                    enemy.attackPower, attackBonus, healBonus);
                
                if (enemy.playerFirst && step == 0)
                    battleLog += "\n[Enemy goes first]\n";
                
                ApplyStepResult(ref playerHp, ref enemyHp, stepResult, ref battleLog);
                
                if (playerHp <= 0)
                {
                    battleLog += "\n[FAIL] Signal intercepted. Game Over.\n";
                    roguelikeProgress.ForceKill();
                    return BattleResult.Defeat;
                }
                
                if (enemyHp <= 0)
                {
                    battleLog += "\n[WIN] Enemy defeated. Signal delivered.\n";
                    SyncFinalHP(roguelikeProgress, playerHp);
                    return BattleResult.Victory;
                }
            }

            // Battle finished after 3 steps
            SyncFinalHP(roguelikeProgress, playerHp);
            
            if (playerHp > 0 && enemyHp > 0)
            {
                battleLog += "\nBattle finished, signal is on its way... Ally received the message.";
            }
            
            return BattleResult.Victory;
        }

        // Apply result of one step
        private static void ApplyStepResult(ref int playerHp, ref int enemyHp, StepResult result, ref string battleLog)
        {
            int oldPlayerHp = playerHp;
            int oldEnemyHp = enemyHp;
            
            playerHp += result.playerHeal;
            playerHp -= result.enemyDamage;
            enemyHp -= result.playerDamage;
            
            playerHp = Mathf.Max(0, playerHp);
            enemyHp = Mathf.Max(0, enemyHp);
            
            // Краткий лог действия
            string actionLog = "";
            
            if (result.playerHeal > 0)
                actionLog += $" Heal +{result.playerHeal}";
            if (result.playerDamage > 0)
                actionLog += $" Attack -{result.playerDamage}";
            if (result.enemyDamage > 0)
                actionLog += $" Enemy -{result.enemyDamage}";
            
            battleLog += $"\nStep: {result.logMessage}{actionLog}";
            battleLog += $"\nHP {oldPlayerHp}|{playerHp} | Enemy {oldEnemyHp}|{enemyHp}\n";
        }

        // Sync final HP with RoguelikeProgress
        private static void SyncFinalHP(RoguelikeProgress progress, int finalHp)
        {
            if (progress != null)
            {
                int beforeHp = progress.CurrentHP;
                Debug.Log($"[SyncFinalHP] ENTER: finalHp={finalHp}, progress.CurrentHP={beforeHp}");
                
                progress.CurrentHP = finalHp;
                
                Debug.Log($"[SyncFinalHP] AFTER: progress.CurrentHP={progress.CurrentHP}");
                progress.UpdateUI();
            }
            else
            {
                Debug.LogError("[SyncFinalHP] progress = null!");
            }
        }
    }
}