using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnTimerUnpause")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnTimerUnpause", message: "Unpause", category: "Events", id: "c20444a7b0e5ee4043861eea786cc0f4")]
public sealed partial class OnTimerUnpause : EventChannel { }

