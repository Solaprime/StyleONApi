using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleONApi.AuthServices
{
    public interface IDispatchService
    {
        Task<UserManagerResponse> PickOrder(Guid orderId);
    }
}
// Pick Order 