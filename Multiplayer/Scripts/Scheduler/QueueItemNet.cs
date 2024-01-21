using Assets.Module.Multiplayer.Scripts.Scheduler;
using System;
using System.Collections.Generic;
using System.Net;
using UpdServerCore.Protocols;

namespace Assets.Multiplayer.Scheduler
{
	/// <summary>Элемент планировщика.</summary>
	public sealed class QueueItemNet
	{
		public Action Action { get; }

		public RulesSending Type { get; }

		public string Name { get; }

		public DateTime? DateTime { get; set; }

		public List<EndPoint> Ignored { get; set; } = new List<EndPoint>();

		public List<EndPoint> Allow { get; set; }

		public QueueItemNet(
            Action action,
			RulesSending type,
			string name) 
		{ 
			Action = action;
			Type = type;
			Name = name;
		}
	}
}
