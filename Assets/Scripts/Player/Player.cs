using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCameraController Camera { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Animator = GetComponent<Animator>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCameraController>();
    }
}
