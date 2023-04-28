using UnityEngine;

namespace Unity2dPlatformerCookbook.Scripts.Entities.Data
{
    [CreateAssetMenu(fileName="JumpConfiguration", menuName="EntityConfigurations/JumpConfiguration", order=2)]
    public class JumpConfiguration : ScriptableObject
    {
        public LayerMask GroundLayer;
        public float JumpSpeed;
        public bool AirJump;
        public int MaxAirJumps;
        public float AirJumpWindow;
        public bool AerialMove;
        public float GroundDistance;
    }
}