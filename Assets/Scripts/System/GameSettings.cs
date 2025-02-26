using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private void Awake()
    {
        // Set the target frame rate to 60 frames per second.
        Application.targetFrameRate = 60;
        // Force Application Resolution to 1920x1080.
        Screen.SetResolution(1920, 1080, true);
    }
}
