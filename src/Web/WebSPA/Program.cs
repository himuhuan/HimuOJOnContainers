// BFF & Gateway for SPA
// This project is a part of HimuOJ
// Copyright (C) 2024 Himu, all rights reserved.

#region

using HimuOJ.Common.WebHostDefaults;
using HimuOJ.Web.WebSPA;

#endregion

const string API_NAME    = "WebSPA";
const string API_VERSION = "v1";

AppHostDefaults.CreateBootstrapBuilder(args, API_NAME, API_VERSION)
    .ConfigureServices()
    .ConfigurePipeline()
    .Run();