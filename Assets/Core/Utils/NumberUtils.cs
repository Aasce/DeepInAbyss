using UnityEngine;

namespace Asce.Managers.Utils
{
    public static class NumberUtils
    {
        public static int GetIntegerDigitCount(float number)
        {
            number = Mathf.Abs(number);
            int intPart = (int)Mathf.Floor(number);

            if (intPart == 0)
                return 1;

            return (int)Mathf.Floor(Mathf.Log10(intPart)) + 1;
        }

    }
}