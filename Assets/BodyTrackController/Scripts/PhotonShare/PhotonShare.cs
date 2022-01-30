using System;
using System.Threading;
using BodyTrackController.Scripts.AnimatorBone;
using BodyTrackController.Scripts.ARTracking;
using BodyTrackController.Scripts.Extensions;
using BodyTrackController.Scripts.MessagePack;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if PHOTON_UNITY_NETWORKING
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
#endif

namespace BodyTrackController.Scripts.PhotonShare
{
    public class PhotonShare : MonoBehaviour
    {
#if PHOTON_UNITY_NETWORKING

        #region Initialize

        private const string RoomName = nameof(RoomName);

        private void Start()
        {
            Init(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid Init(CancellationToken token)
        {
            await InitializePhoton(token);

#if UNITY_IOS && !UNITY_EDITOR
            if (bodyTracker != null)
            {
                SendLoop(token).Forget();
                return;
            }
#endif

            RegisterReceive(token).Forget();
        }

        private static async UniTask InitializePhoton(CancellationToken token)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }

            await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady, cancellationToken: token);

            if (!PhotonNetwork.InRoom)
            {
                var roomOptions = new RoomOptions();
                PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
            }
            await UniTask.WaitUntil(() => PhotonNetwork.InRoom, cancellationToken: token);
        }

        #endregion

        private const int EventCodeSharePose = 11;

        #region Send

        [SerializeField]
        private BodyTracker bodyTracker;

        private async UniTaskVoid SendLoop(CancellationToken token)
        {
            if (bodyTracker == null)
            {
                return;
            }

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(1000 / PhotonNetwork.SendRate), cancellationToken: token);
                await UniTask.SwitchToMainThread();
                Send();
            }
        }

        private void Send()
        {
            var poseAccessor = bodyTracker.HumanPoseAccessor;
            if (poseAccessor == null)
            {
                return;
            }

            PhotonNetwork.RaiseEvent(
                EventCodeSharePose,
                poseAccessor.GetCurrentPose().ToCompress().Serialize(),
                RaiseEventOptions.Default,
                SendOptions.SendUnreliable);
        }

        #endregion

        #region Receive

        [SerializeField]
        private HumanPoseAccessor humanPoseAccessor;

        private async UniTaskVoid RegisterReceive(CancellationToken token)
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
            await UniTask.WaitUntilCanceled(token);
            PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
        }

        private void OnEventReceived(EventData data)
        {
            if (data.Code != EventCodeSharePose)
            {
                return;
            }
            var buffer = (byte[])data.CustomData;
            var humanPose = HumanPoseCompress.Deserialize(buffer).HumanPose;
            humanPoseAccessor.SetPose(ref humanPose);
        }

        #endregion

#endif
    }
}
