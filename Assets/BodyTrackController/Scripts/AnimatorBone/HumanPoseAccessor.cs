using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace BodyTrackController.Scripts.AnimatorBone
{
    public sealed class HumanPoseAccessor : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        private HumanPoseHandler _humanPoseHandler;
        private HumanPose _humanPose;

        public HumanPose HumanPose => _humanPose;
        public Transform Ts { get; private set; }

        private void OnEnable()
        {
            if (animator == null || !animator.isHuman)
            {
                enabled = false;
                return;
            }

            Ts = transform;
            var animTs = animator.transform;
            if (animTs.parent != Ts)
            {
                animTs.parent = Ts;
            }
            animTs.localPosition = Vector3.zero;
            animTs.localRotation = Quaternion.identity;
            animTs.localScale = Vector3.one;

            _humanPose = new HumanPose();
            _humanPoseHandler = new HumanPoseHandler(animator.avatar, animTs);
        }

        private void OnDisable()
        {
            _humanPoseHandler?.Dispose();
            _humanPoseHandler = null;
            _humanPose = new HumanPose();
        }

        public HumanPose GetCurrentPose()
        {
            _humanPoseHandler.GetHumanPose(ref _humanPose);
            return HumanPose;
        }

        public void SetPose(ref HumanPose humanPose)
        {
            _humanPoseHandler.SetHumanPose(ref humanPose);
        }
    }
}
