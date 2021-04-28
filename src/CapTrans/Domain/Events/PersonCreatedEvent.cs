using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapTrans
{
    public class PersonCreatedEvent : INotification
    {
        public Person Person { get; set; }
    }
}
