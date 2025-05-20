using System;

namespace Asce.Game
{
    public static class EnumUtils
    {
        public static T GetTypeByDirection<T>(float direction) where T : Enum
        {
            int value = 0;
            if (direction > 0.0f) value = 1;
            else if (direction < 0.0f) value = -1;

            // Convert int to enum
            if (Enum.IsDefined(typeof(T), value))
                return (T)Enum.ToObject(typeof(T), value);

            throw new ArgumentException($"No matching enum value for {value} in {typeof(T).Name}");
        }

        public static FacingType GetFacingByDirection(float direction)
        {
            return GetTypeByDirection<FacingType>(direction);
        }

        public static bool IsSameDirection(this FacingType self, float direction)
        {
            FacingType facing = GetFacingByDirection(direction);
            return self == facing;
        }
    }
}