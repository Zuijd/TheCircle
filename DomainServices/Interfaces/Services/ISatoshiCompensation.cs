using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Interfaces.Services
{
    public interface ISatoshiCompensation
    {
        decimal CalculateCompensation(TimeSpan streamDuration);
    }
}
