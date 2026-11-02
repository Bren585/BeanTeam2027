using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TimeUp")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TimeUp", message: "Time Up", category: "Events", id: "927a194bd41346b881f0d07ea680b408")]
public sealed partial class TimeUp : EventChannel { }

