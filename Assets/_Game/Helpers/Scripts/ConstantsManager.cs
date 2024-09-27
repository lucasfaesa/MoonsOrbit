using System;

namespace Helpers
{
    public static class ConstantsManager
    {
        public enum TargetType {NONE, HUMAN, MONSTER, METAL}

        public const string PLAYER_LAYER = "Player";
        public const string PLAYER_HITDETECTION_COLLIDER_LAYER = "PlayerHitDetectionCollider";
        public const string NETWORK_PLAYER_LAYER = "NetworkPlayer";
        public const string NETWORK_PLAYER_HITDETECTION_COLLIDER_LAYER = "NetworkPlayerHitDetectionCollider";
        public const string ENEMY_LAYER = "Enemy";
        public const string PHYSICAL_BULLET_LAYER = "PhysicalBullet";
        
        
    }    
}

