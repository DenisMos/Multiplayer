using System;
using System.Reflection;
using System.Text;

namespace Assets.Multiplayer.Framework
{
	public static class TypeTable
	{
		public static byte[] ConvertToData(object field, byte typeData)
		{
			byte[] data = null;
			switch(typeData)
			{
				case 0x10:
					data = Encoding.Default.GetBytes(field.ToString());
					break;
				case 0xA2:
					data = BitConverter.GetBytes((bool)field);
					break;
				case 0xA0:
					data = BitConverter.GetBytes((int)field);
					break;
				case 0xA1:
					data = BitConverter.GetBytes((float)field);
					break;
				case 0xB0:
					data = BitConverter.GetBytes((long)field);
					break;
				case 0xB1:
					data = BitConverter.GetBytes((double)field);
					break;
			}

			return data;
		}

		public static byte[] ConvertToData(object field)
		{
			byte[] data = null;
			if(field is string str)
			{
				data = Encoding.Default.GetBytes(field.ToString());
			}
			else if(field is bool @bool)
			{
				data = BitConverter.GetBytes(@bool);
			}
			else if(field is int int32)
			{
				data = BitConverter.GetBytes(int32);
			}
			else if(field is float int32f)
			{
				data = BitConverter.GetBytes(int32f);
			}
			else if(field is long int64)
			{
				data = BitConverter.GetBytes((long)field);
			}
			else if(field is double int64d)
			{
				data = BitConverter.GetBytes(int64d);
			}

			return data;
		}

		public static object ConvertDataToObject(byte typeData, byte[] data)
		{
			object obj = default(object);
			switch(typeData)
			{
				case 0x10:
					obj = Encoding.Default.GetString(data);
					break;
				case 0xA2:
					obj = BitConverter.ToBoolean(data, 0);
					break;
				case 0xA0:
					obj = BitConverter.ToInt32(data, 0);
					break;
				case 0xA1:
					obj = BitConverter.ToSingle(data, 0);
					break;
				case 0xB0:
					obj = BitConverter.ToInt64(data, 0);
					break;
				case 0xB1:
					obj = BitConverter.ToDouble(data, 0);
					break;
			}

			return obj;
		}

		public static byte TransformTypeByte => 0xF0;

		public static byte GetTypeData(FieldInfo fieldInfo)
		{
			if(fieldInfo.FieldType == typeof(string))
			{
				return 0x10;
			}
			else if(fieldInfo.FieldType == typeof(bool))
			{
				return 0xA2;
			}
			else if(fieldInfo.FieldType == typeof(int))
			{
				return 0xA0;
			}
			else if(fieldInfo.FieldType == typeof(float))
			{
				return 0xA1;
			}
			else if(fieldInfo.FieldType == typeof(long))
			{
				return 0xB0;
			}
			else if(fieldInfo.FieldType == typeof(double))
			{
				return 0xB1;
			}

			return 0x00;
		}

		public static byte GetTypeData(object field)
		{
			if(field is string)
			{
				return 0x10;
			}
			else if(field is bool)
			{
				return 0xA2;
			}
			else if(field is int)
			{
				return 0xA0;
			}
			else if(field is float)
			{
				return 0xA1;
			}
			else if(field is long)
			{
				return 0xB0;
			}
			else if(field is double)
			{
				return 0xB1;
			}

			return 0x0;
		}
	}
}
