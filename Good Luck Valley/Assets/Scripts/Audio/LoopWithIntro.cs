using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopWithIntro : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] AudioSource musicIntro;
    [SerializeField] AudioSource musicLoop;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        musicIntro.Play();
        musicLoop.PlayDelayed(musicIntro.clip.length);
    }
}
