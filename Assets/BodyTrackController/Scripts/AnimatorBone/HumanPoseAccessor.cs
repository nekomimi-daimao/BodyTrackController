using UnityEngine;

namespace BodyTrackController.Scripts.AnimatorBone
{
    [RequireComponent(typeof(Animator))]
    public sealed class HumanPoseAccessor : MonoBehaviour
    {
        private Animator _animator;
        private HumanPoseHandler _humanPoseHandler;
        private HumanPose _humanPose;

        public HumanPose HumanPose => _humanPose;
        public Transform ParentTs { get; private set; }

        private const string ParentObjectName = nameof(HumanPoseAccessor) + "Parent";

        private void OnEnable()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            if (!_animator.isHuman)
            {
                enabled = false;
                return;
            }

            var ts = transform;
            if (ts.parent == null)
            {
                ts.parent = new GameObject(ParentObjectName).transform;
            }
            ParentTs = ts.parent;
            ts.localPosition = Vector3.zero;
            ts.localRotation = Quaternion.identity;
            ts.localScale = Vector3.one;

            _humanPose = new HumanPose();
            _humanPoseHandler = new HumanPoseHandler(_animator.avatar, _animator.transform);
        }

        private void OnDisable()
        {
            _humanPoseHandler?.Dispose();
            _humanPoseHandler = null;
            _humanPose = new HumanPose();
        }

        private void Update()
        {
            _humanPoseHandler.GetHumanPose(ref _humanPose);
        }

        public void SetHumanPose(ref HumanPose humanPose)
        {
            _humanPoseHandler.SetHumanPose(ref humanPose);
        }
    }
}
