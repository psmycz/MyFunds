using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class BaseService<TService> : IBaseService<TService> where TService : class
    {
        // TODO: add logger like this one
        //
        //protected readonly ILogger<TService> _logger;
        //public BaseService(ILogger<TService> logger)
        //{
        //    _logger = logger;
        //}
    }
}
