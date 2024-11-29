using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqPerformance.Model
{
    public record User(int Id, string Name, DateTime CreatedAt, List<Order> Orders);

}
