using UnityEngine;

namespace GoodLuckValley.World.Triggers
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BaseTrigger : MonoBehaviour
    {
        protected virtual void Awake() { }
    }
}
