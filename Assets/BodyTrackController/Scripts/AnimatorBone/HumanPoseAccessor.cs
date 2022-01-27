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

        public bool Initialized => _humanPoseHandler != null && animator != null && animator.isHuman;
        public HumanPose HumanPose => _humanPose;
        public Transform Ts { get; private set; }
        public Animator Animator => animator;

        private void OnEnable()
        {
            if (animator == null)
            {
                return;
            }
            Init(animator);
        }

        private void OnDisable()
        {
            _humanPoseHandler?.Dispose();
            _humanPoseHandler = null;
            _humanPose = new HumanPose();
        }

        public void Init(Animator humanAnimator)
        {
            animator = humanAnimator;
            if (animator == null || !animator.isHuman)
            {
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
