using BodyTrackController.Scripts.MessagePack;
using UnityEngine;

namespace BodyTrackController.Scripts.Extensions
{
    public static class ExtensionHumanPose
    {
        public static HumanPoseSerializable ToSerializable(this HumanPose humanPose)
        {
            return new HumanPoseSerializable(humanPose.bodyPosition, humanPose.bodyRotation, humanPose.muscles);
        }

        public static HumanPoseCompress ToCompress(this HumanPose humanPose)
        {
            return HumanPoseCompress.Parse(ref humanPose);
        }
    }
}
