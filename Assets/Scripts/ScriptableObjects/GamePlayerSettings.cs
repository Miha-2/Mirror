using UnityEngine;

[CreateAssetMenu(fileName = "New GamePlayer Settings", menuName = "Player Settings/Main GamePlayer Settings")]
public class GamePlayerSettings : ScriptableObject
{
    public float movementSpeed = 5f;
    public float sprintSpeed = 1.5f;
    [Range(-180f, 0f)] public float cameraVerticalMin = -70f;
    [Range(0f, 180f)] public float cameraVerticalMax = 70f;
}