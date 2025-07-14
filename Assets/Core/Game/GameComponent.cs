using UnityEngine;

namespace Asce.Managers
{
    public abstract class GameComponent : MonoBehaviour
    {
        protected virtual void Reset()
        {
            this.RefReset();
        }

        protected virtual void RefReset()
        {

        }
    }
}
