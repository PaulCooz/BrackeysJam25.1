using UnityEngine;

namespace JamSpace
{
    public static class Utils
    {
        public static Rect GetWorldRect(this RectTransform rt)
        {
            var rect = rt.rect;
            var xMin = rect.x;
            var yMin = rect.y;
            var xMax = rect.xMax;
            var yMax = rect.yMax;

            var ltw        = rt.localToWorldMatrix;
            var leftBottom = ltw.MultiplyPoint(new Vector3(xMin, yMin, 0.0f));
            var rightTop   = ltw.MultiplyPoint(new Vector3(xMax, yMax, 0.0f));
            var width      = rightTop.x - leftBottom.x;
            var height     = rightTop.y - leftBottom.y;

            return new Rect(leftBottom, new Vector2(width, height));
        }

        public static float Sqr(float a) => a * a;

        public static float DistXY(this Vector3 a, Vector3 b) => Mathf.Sqrt(Sqr(a.x - b.x) + Sqr(a.y - b.y));

        public static Color WithA(this Color c, float alpha)
        {
            var n = c;
            n.a = alpha;
            return n;
        }
    }
}