using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Infrastructure.Services
{
    public class BaseService<T> where T : class
    {
        public IMapper Mapper { get; set; }
        public ILogger<T> Logger { get; set; }
        private readonly IMapper _mapper;
        private ILogger<T> _logger;
        public BaseService(IMapper mapper, ILogger<T> logger )
        {
            _mapper = mapper;
            _logger = logger;
            Logger = _logger;
            Mapper = _mapper;
        }
    }
}
