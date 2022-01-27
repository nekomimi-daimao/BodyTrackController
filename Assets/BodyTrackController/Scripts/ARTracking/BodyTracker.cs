using System.Linq;
using BodyTrackController.Scripts.AnimatorBone;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.XR.ARSubsystems;

namespace BodyTrackController.Scripts.ARTracking
{
    public class BodyTracker : MonoBehaviour
    {
        private ARHumanBodyManager _humanBodyManager;

        private BoneController _boneController;
        private TrackableId _trackableId = TrackableId.invalidId;

        [SerializeField]
        private HumanPoseAccessor humanPoseAccessorPrefab;

        public HumanPoseAccessor HumanPoseAccessor { get; private set; }

        private void OnEnable()
        {
            if (_humanBodyManager == null)
            {
                _humanBodyManager = FindObjectOfType<ARHumanBodyManager>();
                if (_humanBodyManager == null)
                {
                    var sessionOrigin = FindObjectOfType<ARSessionOrigin>();
                    if (sessionOrigin == null)
                    {
                        this.enabled = false;
                        return;
                    }
                    _humanBodyManager = sessionOrigin.gameObject.AddComponent<ARHumanBodyManager>();
                }
            }

            _humanBodyManager.pose2DRequested = false;
            _humanBodyManager.pose3DRequested = true;
            _humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }

        private void OnDisable()
        {
            if (_humanBodyManager != null)
            {
                _humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
            }
            Clear();
        }

        private void Clear()
        {
            if (HumanPoseAccessor != null)
            {
                Destroy(HumanPoseAccessor);
            }
            HumanPoseAccessor = null;
            _boneController = null;
            _trackableId = TrackableId.invalidId;
        }

        private void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs arg)
        {
            if (arg.added.Count > 0)
            {
                var arHumanBodyAdd = arg.added[0];
                _trackableId = arHumanBodyAdd.trackableId;
                HumanPoseAccessor = Instantiate(humanPoseAccessorPrefab, arHumanBodyAdd.transform);
                _boneController = HumanPoseAccessor.gameObject.GetComponentInChildren<BoneController>();
                _boneController.InitializeSkeletonJoints();
                _boneController.ApplyBodyPose(arHumanBodyAdd);
            }

            if (arg.removed.Count > 0 && arg.removed.Any(body => body.trackableId == _trackableId))
            {
                Clear();
            }

            if (arg.updated.Count == 0 || _boneController == null)
            {
                return;
            }
            _boneController.ApplyBodyPose(arg.updated[0]);
        }
    }
}
