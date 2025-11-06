using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Core.Interfaces__Ports_
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
