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
        public float VisibleRadius = 2;
        public float MaxAcceleration = 5;
        public int MaxHitCollider = 100;
        public float MaxSpeed = 5;
        public float MomChickPullForce = 1;
        public AnimationCurve AvoidObstacleFallOut;
        public AnimationCurve AvoidWallFallOut;
        [FormerlySerializedAs("DistObstacleFallOut")] public AnimationCurve DistWallFallOut;
        [FormerlySerializedAs("InvertFallOut")] public AnimationCurve SeparationFallOut;
        [FormerlySerializedAs("NormalFallOut")] public AnimationCurve CohesionFallOut;
        public AnimationCurve AlignmentFallout;
        public AnimationCurve MomChickEffectFallOut;
        public HashSet<Agent> Agents { get; set; } = new HashSet<Agent>();
        
    }
}