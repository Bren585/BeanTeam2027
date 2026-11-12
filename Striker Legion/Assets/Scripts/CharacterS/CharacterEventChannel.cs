using System;

static class CharacterEventChannel
{
    public static event Action ShootBegan;
    public static event Action ShootEnded;

    public static void RaiseShootBegin() => ShootBegan?.Invoke();
    public static void RaiseShootEnd() => ShootEnded?.Invoke();
}