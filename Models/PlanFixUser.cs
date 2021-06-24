using System;
using System.Collections.Generic;
using System.Text;

namespace StopHandler.Models
{
    class PlanFixUser
    {
        public Guid Id { get; }
        public string Name { get; }

        public PlanFixUser(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
