using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.Data
{
    [CreateAssetMenu(fileName="AttackConfiguration", menuName="EntityConfigurations/AttackConfiguration", order=3)]
    public class AttackConfiguration : ScriptableObject
    {
        public bool AttackMove;
    }
}