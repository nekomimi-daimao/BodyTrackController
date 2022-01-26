using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;

namespace BodyTrackController.Scripts.ARTracking
{
    public class BodyTracker : MonoBehaviour
    {
        private ARHumanBodyManager _humanBodyManager;

        [SerializeField]
        private BoneController boneController;

        private void OnEnable()
        {
            if (boneController == null)
            {
                this.enabled = false;
                return;
            }

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

            boneController.InitializeSkeletonJoints();
            _humanBodyManager.humanBodiesChanged += OnHumanBodiesChanged;
        }

        private void OnDisable()
        {
            if (_humanBodyManager != null)
            {
                _humanBodyManager.humanBodiesChanged -= OnHumanBodiesChanged;
            }
        }

        private void OnHumanBodiesChanged(ARHumanBodiesChangedEventArgs arg)
        {
            if (arg.updated.Count == 0)
            {
                return;
            }
            boneController.ApplyBodyPose(arg.updated[0]);
        }
    }
}
