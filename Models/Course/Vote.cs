using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Models.Course
{
    public record Vote
    {
        public int Stars { get; init; }
        public Guid VoterId { get; init; }
    }
}
