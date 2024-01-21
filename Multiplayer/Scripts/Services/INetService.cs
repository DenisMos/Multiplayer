using UdpServerCore.Core;

namespace Assets.Module.Multiplayer.Scripts.Services
{
	public interface INetService<TData>
	{
		void CallResponse(ResponseData responseData, TData data, bool verb = false);
	}
}
