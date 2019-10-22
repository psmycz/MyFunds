using AutoMapper;
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
        protected readonly IMapper mapper;


        public BaseService(IMapper mapper)
        {
            this.mapper = mapper;
        }


    }
}
