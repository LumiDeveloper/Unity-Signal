// LUMI :)
using System;
using UnityEngine;

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
        public Sprite enemySprite;
        
        // Конструктор для простого врага
        public EnemyData(string name, ShapeType[] shapes, bool playerFirst, int hp, int attack, Sprite sprite)
        {
            this.enemyName = name;
            this.shapes = shapes;
            this.playerFirst = playerFirst;
            this.enemyHp = hp;
            this.attackPower = attack;
            this.enemySprite = sprite;  // <-- добавить эту строку
        }
    }
}