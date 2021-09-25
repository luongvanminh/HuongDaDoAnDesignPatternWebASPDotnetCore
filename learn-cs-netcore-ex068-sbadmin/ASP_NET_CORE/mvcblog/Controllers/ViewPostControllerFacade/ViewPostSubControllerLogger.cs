using System;
using Microsoft.Extensions.Logging;

namespace mvcblog.Controllers.ViewPostControllerFacade
{
    public class ViewPostSubControllerLogger
    {
        private readonly ILogger<ViewPostController> _logger;

        public ViewPostSubControllerLogger(ILogger<ViewPostController> logger)
        {
            _logger = logger;
        }

        public void PrintRoutes()
        {
            _logger.LogInformation($@"{GetType().Name}
                Routes:
                /posts
                :slug/listpost
                :slug.html/viewonepost
                ");
        }
    }
}
