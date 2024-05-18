using Combat;
using Game.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Queue
{
    public class Marble : IQueueable
    {
        public float InQueueSpeed { get; set; }
        public float CurrentWaitingTime { get; set; }
        public int CurrentGoalIndex { get; set; }

        public ProjectileModel ProjectileModel { get; private set; }

        public Marble(float inQueueSpeed, ProjectileModel projectileModel)
        {
            InQueueSpeed = inQueueSpeed;
            ProjectileModel = projectileModel;
        }
    }
}