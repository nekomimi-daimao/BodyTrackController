using System;
using MessagePack;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace BodyTrackController.Scripts.MessagePack
{
    [MessagePackObject, Serializable]
    public readonly struct HumanPoseSerializable
    {
        [Key(0)]
        public readonly Vector3 BodyPosition;

        [Key(1)]
        public readonly Quaternion BodyRotation;

        [Key(2)]
        public readonly float[] Muscles;

        public HumanPoseSerializable(Vector3 bodyPosition, Quaternion bodyRotation, float[] muscles)
        {
            BodyPosition = bodyPosition;
            BodyRotation = bodyRotation;
            Muscles = muscles;
        }

        public HumanPose HumanPose => new HumanPose
        {
            bodyPosition = BodyPosition,
            bodyRotation = BodyRotation,
            muscles = Muscles,
        };

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public static HumanPoseSerializable Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<HumanPoseSerializable>(buffer);
        }
    }
}
