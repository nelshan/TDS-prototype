using UnityEngine;

public class Enemy5SpawnedObjectcounter : MonoBehaviour
{
    public delegate void DestroyedHandler();
    public event DestroyedHandler OnDestroyed;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
