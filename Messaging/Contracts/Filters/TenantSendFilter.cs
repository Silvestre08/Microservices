using MassTransit;
using MassTransit.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Filters
{
    public class TenantSendFilter<T> : IFilter<SendContext<T>> where T : class
    {
        private readonly Tenant _tenant;

        public TenantSendFilter(Tenant tentant)
        {
            _tenant = tentant ?? throw new ArgumentNullException(nameof(tentant));
        }

        public void Probe(ProbeContext context)
        {
            throw new NotImplementedException();
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (!string.IsNullOrWhiteSpace(_tenant.TenantId)) 
            {
                context.Headers.Set("Tennant-From-App", _tenant.TenantId);
            }

            return next.Send(context);
        }
    }
}
