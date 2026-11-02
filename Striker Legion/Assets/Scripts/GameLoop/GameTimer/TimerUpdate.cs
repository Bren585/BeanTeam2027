using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TimerUpdate")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TimerUpdate", message: "[SetActive]", category: "Events", id: "2222643119564a1bdbb430b50fdd7a28")]
public sealed partial class TimerUpdate : EventChannel<bool> { }

