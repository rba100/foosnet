using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoosNet.Vision
{
    public interface ITableWatcher
    {
        bool TableIsInUse { get; }
        event EventHandler TableHasBecomeInUse;
        event EventHandler TableHasBecomeFree;
    }
}