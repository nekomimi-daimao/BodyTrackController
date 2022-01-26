using System;
using MessagePack;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace BodyTrackController.Scripts.MessagePack
{
    [MessagePackObject, Serializable]
    public readonly struct HumanPoseCompress
    {
        [Key(0)]
        public readonly Vector3 BodyPosition;

        [Key(1)]
        public readonly Quaternion BodyRotation;

        [Key(2)]
        public readonly short[] Muscles;

        public HumanPoseCompress(Vector3 bodyPosition, Quaternion bodyRotation, short[] muscles)
        {
            BodyPosition = bodyPosition;
            BodyRotation = bodyRotation;
            Muscles = muscles;
        }

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public static HumanPoseCompress Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<HumanPoseCompress>(buffer);
        }

        private const int Ratio = 100;
        private const float ToShort = Ratio;
        private const float ToFloat = 1f / Ratio;

        public HumanPose HumanPose
        {
            get
            {
                var musclesFloat = new float[Muscles.Length];
                for (var index = 0; index < Muscles.Length; index++)
                {
                    musclesFloat[index] = Muscles[index] * ToFloat;
                }

                return new HumanPose
                {
                    bodyPosition = BodyPosition,
                    bodyRotation = BodyRotation,
                    muscles = musclesFloat
                };
            }
        }

        public static HumanPoseCompress Parse(ref HumanPose pose)
        {
            var musclesFloat = pose.muscles;
            var musclesShort = new short[musclesFloat.Length];
            for (var index = 0; index < musclesFloat.Length; index++)
            {
                musclesShort[index] = (short)(musclesFloat[index] * ToShort);
            }
            return new HumanPoseCompress(pose.bodyPosition, pose.bodyRotation, musclesShort);
        }
    }
}
