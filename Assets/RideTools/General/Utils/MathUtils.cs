using UnityEngine;

namespace RideTools
{
    public static class MathUtils
    {
        public static bool IsPointOnVector(Vector3 originPos, Vector3 targetPos, Vector3 point, float threshold = 1e-6f)
        {
            Vector3 rayDirection = Vector3.Normalize(targetPos - originPos);
            Vector3 cameraToPoint = point - originPos;

            if (Vector3.Dot(cameraToPoint, rayDirection) < 0)
            {
                return false;
            }

            float projectionLength = Vector3.Dot(cameraToPoint, rayDirection);
            Vector3 projection = projectionLength * rayDirection;
            Vector3 perpendicularVector = cameraToPoint - projection;
            float distance = perpendicularVector.sqrMagnitude;

            return distance < threshold;
        }

        public unsafe static float FastInverseSquareRoot(float x)
        {
            int i = *(int*)&x;
            i = 0x5f3759df - (i >> 1);
            float y = *(float*)&i;
            return y * (1.5F - 0.5F * x * y * y);
        }
    }
}