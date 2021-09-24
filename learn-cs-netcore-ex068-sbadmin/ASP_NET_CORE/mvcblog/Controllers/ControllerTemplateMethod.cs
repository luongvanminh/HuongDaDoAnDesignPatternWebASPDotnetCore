using System;
using Microsoft.AspNetCore.Mvc;

namespace mvcblog.Controllers
{
    public abstract class ControllerTemplateMethod : Controller
    {
        protected abstract void PrintRoutes();
        protected abstract void PrintDIs();

        // Template method
        public void PrintInformation()
        {
            PrintRoutes();
            PrintDIs();
        }
    }
}
