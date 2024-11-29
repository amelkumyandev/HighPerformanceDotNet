using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqPerformance.Model
{
    public record Order(int Id, int UserId, decimal Amount, DateTime OrderDate);
}
