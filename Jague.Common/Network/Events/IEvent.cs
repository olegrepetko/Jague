using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Bank.Events
{
    public interface IEvent
    {
        bool CanExecute();
        void Execute();
    }
}
