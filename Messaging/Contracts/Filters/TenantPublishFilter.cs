using MassTransit;

namespace Contracts.Filters
{
    public class TenantPublishFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        private readonly Tenant _tenant;

        public TenantPublishFilter(Tenant tenant)
        {
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            if(string.IsNullOrWhiteSpace(_tenant.TenantId))
            {
                context.Headers.Set("Tenant-From-Publish", _tenant.TenantId);
            }

            return next.Send(context);
        }
    }
}
