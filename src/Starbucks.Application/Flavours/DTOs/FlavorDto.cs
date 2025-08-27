using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbucks.Application.Flavours.DTOs
{
    public class FlavorDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; } = [];

    }
}
