using TimeDistortion.Gameplay.Handler;

public class EnemyTarget : CharacterManager
{
    private InputHandler inpHandler;
    public bool isFollowed;

    public void SetValues(InputHandler inp)
    {
        inpHandler = inp;
        isFollowed = false;
    }

    private void OnDestroy()
    {
        if (isFollowed)
            inpHandler.StopLockOn();
    }
}
