using System;

namespace UdpServerCore.Protocols
{
    public interface ISyncData
    {
        /// <summary>Токен синхронизации.</summary>
        Guid TokenSync { get; }

        /// <summary>Идентификатор поля.</summary>
        long FieldId { get; }

        /// <summary>Данные.</summary>
        byte[] FieldData { get; }

        int Type { get; }

        int Proto { get; }
    }
}