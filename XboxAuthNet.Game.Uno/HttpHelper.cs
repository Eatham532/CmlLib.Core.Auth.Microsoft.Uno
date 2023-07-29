using System;
using System.Net.Http;

namespace XboxAuthNet.Game;

public static class HttpHelper
{
    public static Lazy<HttpClient> DefaultHttpClient
        = new Lazy<HttpClient>(() => new HttpClient());
}
