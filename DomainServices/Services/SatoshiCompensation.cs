using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services
{
    public class SatoshiCompensation : ISatoshiCompensation
    {
        private const decimal InitialCompensation = 0.00000001M;
        private const double CompensationMultiplier = 2;

        private readonly ILogger<SatoshiCompensation> _logger;

        public SatoshiCompensation(ILogger<SatoshiCompensation> logger)
        {
            _logger = logger;
        }

        public decimal CalculateCompensation(TimeSpan streamDuration)
        {
            //Calculate the compensation based on the duration

            //number of consecutive hours
            int consecutiveHours = (int)streamDuration.TotalHours;

            // Calculate the compensation

            decimal compensation = InitialCompensation * (decimal)Math.Pow(CompensationMultiplier, consecutiveHours - 1);

            _logger.LogInformation($"calculated {compensation} Satoshi from {consecutiveHours} consecutive hours streamed ");

            return compensation;

        }
    }
}
