using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace MommyChicken
{
    /// <summary>
    /// Manager of all agent
    /// </summary>
    public class AgentManager : Singleton<AgentManager>
    {
        public LayerMask HitLayerMask;
        public float VisibleRadius = 2;
        public float MaxAcceleration = 5;
        public int MaxHitCollider = 100;
        public float MaxSpeed = 5;
        public float QueenPullForce = 1;
        public AnimationCurve AvoidObstacleFallOut;
        public AnimationCurve DistObstacleFallOut;
        [FormerlySerializedAs("InvertFallOut")] public AnimationCurve SeparationFallOut;
        [FormerlySerializedAs("NormalFallOut")] public AnimationCurve CohesionFallOut;
        public AnimationCurve AlignmentFallout;
        public AnimationCurve QueenEffectFallOut;
        public HashSet<Agent> Agents { get; set; } = new HashSet<Agent>();
        
    }
}