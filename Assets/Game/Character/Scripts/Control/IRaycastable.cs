using RPG.Control;
using UnityEditor.Build.Player;

public interface IRaycastable
{
    bool HandleRaycast(PlayerController callingController);
    CursorType GetCursorType();
}

public enum CursorType
{
    None = 0,
    Movement = 1,
    Combat = 2,
    UI = 3,
    pickup = 4,
    fullPickup = 5,
    Dialogue = 6,
    Shop = 7
}