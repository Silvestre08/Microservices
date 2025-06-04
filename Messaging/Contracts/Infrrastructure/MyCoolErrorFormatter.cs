using MassTransit;

namespace Contracts.Infrrastructure
{
    public class MyCoolErrorFormatter : IErrorQueueNameFormatter
    {
        public string FormatErrorQueueName(string queueName)
        {
            return queueName + "-awesome_error";
        }
    }
}
