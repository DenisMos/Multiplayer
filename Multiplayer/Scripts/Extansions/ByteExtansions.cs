namespace Assets.Multiplayer.Scripts.Extansions
{
	public static class ByteExtansions
	{
		public static bool EqualsArray(this byte[] source, byte[] data)
		{
			if(source.Length != data.Length)
			{
				return false;
			}

			int i = 0;
			while(i < source.Length)
			{
				if(source[i] != data[i])
				{
					return false;
				}
				i++;
			}

			return true;
		}
	}
}
