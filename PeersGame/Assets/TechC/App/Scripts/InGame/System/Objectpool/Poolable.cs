using UnityEngine;

namespace TechC.InGame.ObjectPool
{
    public interface IPoolable
    {
        void OnPoolGet();
        void OnPoolReturn();
    }
}
