namespace Services
{
    public interface IRabbitService
    {
        void SendMessage();
        void SendMultipleMessages();
    }
}