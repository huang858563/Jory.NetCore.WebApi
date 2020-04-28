using System;
using System.Collections.Generic;
using System.Text;

namespace Jory.NetCore.Model.Entities
{
    public abstract class BaseEntity<TKey>
    {
        public virtual TKey Id { get; set; }
    }
}
