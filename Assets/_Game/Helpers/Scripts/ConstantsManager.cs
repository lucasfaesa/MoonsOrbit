using System;

namespace Helpers
{
    public static class ConstantsManager
    {
        public enum TargetType {NONE, HUMAN, ENEMY, METAL}

        public const string PLAYER_LAYER = "Player";
        public const string PLAYER_HITDETECTION_COLLIDER_LAYER = "PlayerHitDetectionCollider";
        public const string NETWORK_PLAYER_LAYER = "NetworkPlayer";
        public const string NETWORK_PLAYER_HITDETECTION_COLLIDER_LAYER = "NetworkPlayerHitDetectionCollider";
        public const string ENEMY_LAYER = "EnemyCollider";
        public const string PHYSICAL_BULLET_LAYER = "PhysicalBullet";

        public const string ENEMY_TAG = "Enemy";

    }    
}

