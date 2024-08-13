using UnityEngine;

namespace GoodLuckValley.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 Round(this Vector2 v, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10f, decimalPlaces);
            float xValue = Mathf.Round(v.x * multiplier) / multiplier;
            float yValue = Mathf.Round(v.y * multiplier) / multiplier;

            return new Vector2(xValue, yValue);
        }
    }
}