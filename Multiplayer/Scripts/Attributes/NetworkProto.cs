namespace Assets.Multiplayer.Attributes
{
	/// <summary>Сетевые правила.</summary>
	public enum NetworkProto
	{
		/// <summary>Синхронизация поля.</summary>
		Sync,
		/// <summary>Синхронизация Transform.</summary>
		Transform,
		/// <summary>Поле является уникальным номером объекта.</summary>
		NetworkId,
	}

	/// <summary>Сетевые правила для методов.</summary>
	public enum NetworkMethodsProto
	{
		None,
		/// <summary>Объявление общего метода в сети.</summary>
		/// <remarks>При вызове данного метода, он будет выполнен у всех участников сети.</remarks>
		RPC,
	}
}
