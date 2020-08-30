using System;
using System.Collections.Generic;
using System.Text;

namespace Store.DAL
{
    public enum TransactionIsolationLevel
    {
        Default = 0,
        ReadUncommitted = 1,
        ReadCommitted = 2,
        RepeatableRead = 3,
        Snapshot = 4,
        Serializable = 5
    }
}
