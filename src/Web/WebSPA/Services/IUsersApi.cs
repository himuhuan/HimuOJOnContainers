﻿using Identity.Server.Controllers;
using Refit;

namespace HimuOJ.Web.WebSPA.Services;

public interface IUsersApi
{
    [Get("/api/users/{id}/brief")]
    Task<UserBrief> GetUserBriefAsync(string id);

    [Get("/api/users/briefs")]
    Task<IDictionary<string, UserBrief>> GetUserBriefsAsync([Query] GetUserBriefsRequest request);
}