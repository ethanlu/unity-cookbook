using UnityEngine;

namespace Entities.Data
{
    [CreateAssetMenu(fileName="AttackConfiguration", menuName="EntityConfigurations/AttackConfiguration", order=3)]
    public class AttackConfiguration : ScriptableObject
    {
        public bool AttackMove;
    }
}