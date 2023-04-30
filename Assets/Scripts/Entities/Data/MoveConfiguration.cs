using UnityEngine;

namespace Entities.Data
{
    [CreateAssetMenu(fileName="MoveConfiguration", menuName="EntityConfigurations/MoveConfiguration", order=1)]
    public class MoveConfiguration : ScriptableObject
    {
        public bool InstantTopSpeed;
        public float TopSpeed;
        public float AccelerationSpeed;
        public float DecelerationSpeed;
    }
}