using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnTimerPause")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnTimerPause", message: "Pause", category: "Events", id: "04116d7b7e2141387229a2090059cae8")]
public sealed partial class OnTimerPause : EventChannel { }

