using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnGoal")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnGoal", message: "Goal!", category: "Events", id: "5d0937d3883c1bbeed166eeaf73c19b1")]
public sealed partial class OnGoal : EventChannel { }

