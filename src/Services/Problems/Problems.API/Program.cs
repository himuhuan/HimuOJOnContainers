#region

using HimuOJ.Common.WebHostDefaults;
using HimuOJ.Services.Problems.API;

#endregion

const string APP_NAME = "Problems.API";
const string VERSION  = "v1";

AppHostDefaults.CreateBootstrapBuilder(args, APP_NAME, VERSION)
    .ConfigureServices()
    .ConfigurePipeline()
    .Run();