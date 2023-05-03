using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName="AttackConfiguration", menuName="EntityConfigurations/AttackConfiguration", order=3)]
    public class AttackConfiguration : ScriptableObject
    {
        public bool AttackMove;
    }
}