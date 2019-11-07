using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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


        // not 100% sure if it works correctly
        // just check properties like int, bool, string etc. does not load entities from db
        protected bool HasPropertyUpdated<T, TEntity>(int updatedObjectId, T objectDTO, IBaseRepository<TEntity> repository) where TEntity : class
        {
            if (updatedObjectId <= 0)
                throw new ApiException("Incorrect Id");

            var obj = repository.GetById(updatedObjectId);

            if (obj == null) throw new NoDataException($"No {obj.GetType().Name.Replace("Proxy", string.Empty)} with provided Id");

            bool change = false;

            PropertyInfo[] properties = obj.GetType().GetProperties();
            PropertyInfo[] propertiesDTO = objectDTO.GetType().GetProperties();

            // only when both property names matched compare values
            foreach (var p in propertiesDTO)
            {
                var myValDTO = p.GetValue(objectDTO)?.ToString();
                var myVal = properties.FirstOrDefault(prop => prop.Name == p.Name)?.GetValue(obj)?.ToString();

                if (myValDTO == null || myVal == null) continue;

                if (myVal != myValDTO)
                {
                    change = true;
                    break;
                }
            }

            repository.Detach(obj);

            return change;
        }


    }
}
