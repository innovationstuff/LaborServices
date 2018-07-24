using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaborServices.Web.Helpers
{
    public interface IExceptionLogger
    {
        void Log(string message, Exception exception);
    }

    public class DefaultExceptionLogger : IExceptionLogger
    {
        public void Log(string message, Exception exception)
        {
            var logger = log4net.LogManager.GetLogger("FileLogger");
            logger.Error(message, exception);
        }
    }
}
