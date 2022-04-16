using Bhaptics.Tact.Unity;
using UnityEngine;

public class RotationalExample : MonoBehaviour
{
    [Range(0.2f, 5f)]
    public float intensity = 1f;
    [Range(0.2f, 5f)]
    public float duration = 1f;

    [Range(0, 360f)]
    public float angleX;
    [Range(-0.5f, 0.5f)]
    public float offsetY;

    public VestHapticClip clip;


    public void Play()
    {
        if (clip != null)
        {
            clip.Play(intensity, duration, angleX, offsetY);
        }
    }

    private void Update()
    {
        Play();
    }
}
