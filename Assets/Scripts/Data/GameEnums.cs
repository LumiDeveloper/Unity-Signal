// LUMI :)
namespace SignalRoguelite
{
    public enum ShapeType
    {
        Square,     // Парирование
        Triangle,   // Атака
        Circle      // Лечение
    }

    public enum SignalColor
    {
        Red,        // Атакуй
        Green       // Врагов нет
    }

    public enum ColorPosition
    {
        Start,      // Начало (позиция 0)
        Middle,     // Середина (позиция 1)
        End         // Конец (позиция 2)
    }

    public enum BattleResult
    {
        Victory,
        Defeat
    }
}