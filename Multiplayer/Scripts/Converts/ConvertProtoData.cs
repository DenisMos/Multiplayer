using Assets.Multiplayer.Attributes;
using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Serilizator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UdpServerCore.Framework;

namespace Assets.Multiplayer.Scripts.Converts
{
	public static class ConvertProtoData
	{
		public static byte[] Convert(FieldData field, BehaviourContainer bhContainer, SynchronizationContext context)
			=> GetBytes(field, bhContainer, context);
		

		private static byte[] GetBytes(FieldData field, BehaviourContainer bhContainer, SynchronizationContext context)
		{
			switch(field.NetOptEnums)
			{
				case NetworkProto.Transform:
					Vector3 pos = default;
					Quaternion rot = default;
					Vector3 sca = default;

					if(context == null)
					{
						var behaviour = bhContainer.NetworkBehaviour;
						if(bhContainer.NetworkBehaviour != null)
						{
							pos = behaviour.transform.position;
							rot = behaviour.transform.rotation;
							sca = behaviour.transform.localScale;
						}
					}
					else
					{
						context.Send(d =>
						{
							var behaviour = bhContainer.NetworkBehaviour;
							if(bhContainer.NetworkBehaviour != null)
							{
								pos = behaviour.transform.position;
								rot = behaviour.transform.rotation;
								sca = behaviour.transform.localScale;
							}
						}, null);
					}

					return TransformSerialize.Serialize(pos, rot, sca);
				case NetworkProto.Sync:
					var value = field.Value.GetValue(bhContainer.NetworkBehaviour);
					return TypeTable.ConvertToData(value, field.Type);
				default:
					throw new Exception("Unknown protocol!");
			}
		}
	}
}
