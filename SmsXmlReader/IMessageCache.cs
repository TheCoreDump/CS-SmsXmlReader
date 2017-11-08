
namespace SmsXmlReader
{
    public interface IMessageCache
    {
        void CacheMessage<TMessage>(TMessage message)
            where TMessage : MessageBase;

        void Initialize();
    }
}
