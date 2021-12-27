using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Helper.Core
{
    public abstract class ExceptionHandler
    {
        public abstract void HandleException(Exception exception);
    }
}
