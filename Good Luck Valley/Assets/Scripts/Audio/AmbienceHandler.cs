using UnityEngine;
using AK.Wwise;

namespace GoodLuckValley.Audio.Ambience
{
    public class AmbienceHandler : MonoBehaviour
    {
        [Header("Wwise")]
        [SerializeField] private AK.Wwise.Event ambientBed;
        [SerializeField] private AK.Wwise.Event ambient2D;

        private void Start()
        {
            ambientBed.Post(gameObject);
            ambient2D.Post(gameObject);
        }
    }
}