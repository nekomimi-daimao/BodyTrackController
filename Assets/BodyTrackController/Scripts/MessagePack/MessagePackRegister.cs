using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using MessagePack.Unity.Extension;
using UnityEngine;

namespace BodyTrackController.Scripts.MessagePack
{
    public static class MessagePackRegister
    {
        private static bool serializerRegistered = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (serializerRegistered)
            {
                return;
            }

            StaticCompositeResolver.Instance.Register(
                GeneratedResolver.Instance,
                StandardResolver.Instance,
                StaticCompositeResolver.Instance,
                UnityResolver.Instance,
                UnityBlitWithPrimitiveArrayResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
            serializerRegistered = true;
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            Initialize();
        }

#endif
    }
}
