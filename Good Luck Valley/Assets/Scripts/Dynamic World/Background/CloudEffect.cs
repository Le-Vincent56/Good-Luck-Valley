using UnityEngine;

namespace HiveMind.Background
{
    public class CloudEffect : MonoBehaviour
    {
        #region REFERENCES
        private Sprite cloud;
        #endregion

        #region FIELDS
        private float cloudLeftBound;
        private float cloudRightBound;
        private float cloudWidth;
        private float cloudYPos;
        [SerializeField] private float cloudParallaxSpeed;
        #endregion

        #region PROPERTIES
        public float CloudLB { get { return cloudLeftBound; } set { cloudLeftBound = value; } }
        public float CloudRB { get { return cloudRightBound; } set { cloudRightBound = value; } }
        public float CloudWidth { get { return cloudWidth; } set { cloudWidth = value; } }
        public float CloudY { get { return cloudYPos; } set { cloudYPos = value; } }
        public float CloudSpeed { get { return cloudParallaxSpeed; } set { cloudParallaxSpeed = value; } }

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            cloud = GetComponent<SpriteRenderer>().sprite;
            cloudLeftBound = cloud.border.x;
            cloudRightBound = cloud.border.z;
            cloudWidth = cloudRightBound - cloudLeftBound;
            cloudYPos = transform.position.y;
            cloudParallaxSpeed *= 0.001f;
        }
    }
}
