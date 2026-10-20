using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnTimerStart")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnTimerStart", message: "Begin", category: "Events", id: "8c1c94f9e0857d1171dd131a3820d67a")]
public sealed partial class OnTimerStart : EventChannel { }

