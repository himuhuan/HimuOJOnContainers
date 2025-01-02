using HimuOJ.Common.WebHostDefaults;
using HimuOJ.Services.Submits.API.Extensions;

const string APP_NAME = "Submits.API";
const string VERSION  = "v1";

AppHostDefaults.CreateBootstrapBuilder(args, APP_NAME, VERSION)
               .ConfigureServices()
               .ConfigurePipeline()
               .Run();