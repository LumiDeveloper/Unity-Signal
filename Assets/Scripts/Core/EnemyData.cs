// LUMI :)
using System;

namespace SignalRoguelite
{
    [Serializable]
    public class EnemyData
    {
        public string enemyName;
        public ShapeType[] shapes;     // 3 фигуры врага (чёрные, без цвета)
        public bool playerFirst;       // true = игрок ходит первым
        public int enemyHp;            // здоровье врага
        public int attackPower;        // сила атаки врага
        
        // Конструктор для простого врага
        public EnemyData(string name, ShapeType[] shapes, bool playerFirst, int hp, int attack)
        {
            this.enemyName = name;
            this.shapes = shapes;
            this.playerFirst = playerFirst;
            this.enemyHp = hp;
            this.attackPower = attack;
        }
    }
}