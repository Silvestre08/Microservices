using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Filters
{
    public class TenantConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly Tenant _tenant;
        public TenantConsumeFilter(Tenant tenant)
        {
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        public void Probe(ProbeContext context)
        {
            context.CreateMessageScope("TenantConsumeFilter");
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("Tenant-From-Publish", null) ??
                            context.Headers.Get<string>("Tennant-From-App", null);

            return next.Send(context);
        }
    }
}
