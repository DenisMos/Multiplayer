namespace UpdServerCore.Framework
{

    /// <summary>Интерфейс данных.</summary>
    public interface IDataContract<T> : IDataContract
    {
        /// <summary>Уникальный номер хранилища.</summary>
        public byte Id { get; }

        /// <summary>Значение.</summary>
        public T Value { get; }
    }

    public interface IDataContract
    {
        public int Type { get; }

        public byte[] Data { get; }
    }
}
