using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dem2Server
{
    public static class ExceptionIsCriticalCheck
    {
        public static bool IsCritical(Exception ex)
        {
            if (ex is OutOfMemoryException) return true;
            if (ex is AppDomainUnloadedException) return true;
            if (ex is BadImageFormatException) return true;
            if (ex is CannotUnloadAppDomainException) return true;
            if (ex is ExecutionEngineException) return true;
            if (ex is InvalidProgramException) return true;
            if (ex is System.Threading.ThreadAbortException)
                return true;
            return false;
        }
    }
}
