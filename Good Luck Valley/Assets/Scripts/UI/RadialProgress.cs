using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float progressTime;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void UpdateProgress(float progressTotal)
    {
        progressTime = progressTotal;
        image.fillAmount = progressTime;
    }
}
