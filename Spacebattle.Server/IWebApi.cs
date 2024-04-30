
﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using CoreWCF;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace Spacebattle.Server
{

    [ServiceContract]
    [OpenApiBasePath("/spacebattleApi")]
    public interface IWebApi
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/sendMessage")]
        [OpenApiTag("Tag")]
        [OpenApiResponse(ContentTypes = new[] {"application/json"}, Description = "Success", StatusCode = HttpStatusCode.OK, Type = typeof(SpacebattleContract)) ]
        public void SendMessage(
            [OpenApiParameter(ContentTypes = new[] { "application/json" }, Description = "Три обязательных параметрa: type, gameId, gameItemId; и один необязательный - params.")] SpacebattleContract contract);
    }
}  
