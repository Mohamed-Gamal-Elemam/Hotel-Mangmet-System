using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Contracts
{
    public interface IDataIntializer
    {
        Task InitializeDataAsync();
    }
}
