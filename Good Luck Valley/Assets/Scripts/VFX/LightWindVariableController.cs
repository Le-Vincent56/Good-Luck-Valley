using UnityEngine;

namespace GoodLuckValley.VFX
{
    public class LightWindVariableController : MonoBehaviour
    {
        [Header("FREE TO CHANGE")]
        public float windDirection;
        public float bendStrength;

        [Header("DO NOT CHANGE")]
        public float verticalOffset;

        private void Start()
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Bend_Strength", bendStrength);
            propertyBlock.SetFloat("_xDIrection", windDirection);
            propertyBlock.SetFloat("_Vertical_Offset", verticalOffset);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
        }
    }
}