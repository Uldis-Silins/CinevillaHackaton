using UnityEngine;

public class PitchAudio : MonoBehaviour
{
    private void Start()
    {
        GetComponent<AudioSource>().pitch = Random.Range(0.75f, 1.2f);
    }
}
