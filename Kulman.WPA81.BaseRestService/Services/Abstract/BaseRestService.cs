﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using JetBrains.Annotations;
using Kulman.WPA81.BaseRestService.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kulman.WPA81.BaseRestService.Services.Abstract
{
    /// <summary>
    /// Base class for JSON based REST services
    /// </summary>
    public abstract class BaseRestService
    {
        /// <summary>
        /// Http filter
        /// </summary>
        [NotNull]
        private readonly HttpBaseProtocolFilter _filter;

        /// <summary>
        /// Cookie manager
        /// </summary>
        [NotNull]
        protected HttpCookieManager CookieManager => _filter.CookieManager;

        /// <summary>
        /// Logger
        /// </summary>
        [CanBeNull]
        protected ILogger Logger;

        /// <summary>
        /// Ctor, creates Http filter
        /// </summary>
        protected BaseRestService()
        {
            _filter = CreateHttpFilter();
        }

        /// <summary>
        /// Must be overridden to set the Base URL
        /// </summary>
        /// <returns>Base URL</returns>
        protected abstract string GetBaseUrl();

        /// <summary>
        /// Executed before every request
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        protected virtual Task OnBeforeRequest([NotNull] string url, CancellationToken token)
        {
            return Task.FromResult(1);
        }

        /// <summary>
        /// Can be overriden to set the default request headers
        /// </summary>
        /// <returns>Dictionary containing default request headers</returns>
        protected virtual Dictionary<string, string> GetRequestHeaders([NotNull] string requestUrl)
        {
            return new Dictionary<string, string>();
        }

        #region HTTP GET
        /// <summary>
        /// REST Get
        /// </summary>        
        /// <param name="url">Url</param>        
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Get<T>([NotNull] string url)
        {
            return GetResponse<T>(url, HttpMethod.Get, null, CancellationToken.None);
        }

        /// <summary>
        /// REST Get
        /// </summary>        
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Get<T>([NotNull] string url, CancellationToken token)
        {
            return GetResponse<T>(url, HttpMethod.Get, null, token);
        }

        /// <summary>
        /// REST Get (RAW)
        /// </summary>        
        /// <param name="url">Url</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Get([NotNull] string url)
        {
            return GetRawResponse(url, HttpMethod.Get, null, CancellationToken.None);
        }

        /// <summary>
        /// REST Get (RAW)
        /// </summary>        
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Get([NotNull] string url, CancellationToken token)
        {
            return GetRawResponse(url, HttpMethod.Get, null, token);
        }
        #endregion

        #region HTTP DELETE
        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task Delete([NotNull] string url)
        {
            return GetResponse(url, HttpMethod.Delete, null, CancellationToken.None);
        }

        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Delete<T>([NotNull] string url)
        {
            return GetResponse<T>(url, HttpMethod.Delete, null, CancellationToken.None);
        }

        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Delete<T>([NotNull] string url, CancellationToken token)
        {
            return GetResponse<T>(url, HttpMethod.Delete, null, token);
        }

        /// <summary>
        /// REST Delete
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task Delete([NotNull] string url, CancellationToken token)
        {
            return GetResponse(url, HttpMethod.Delete, null, token);
        }
        #endregion

        #region HTTP PUT       
        /// <summary>
        /// REST Put
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Put<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, HttpMethod.Put, request, CancellationToken.None);
        }

        /// <summary>
        /// REST Put
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Put<T>([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetResponse<T>(url, HttpMethod.Put, request, token);
        }

        /// <summary>
        /// REST Put (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Put([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, HttpMethod.Put, request, CancellationToken.None);
        }

        /// <summary>
        /// REST Put (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Put([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetRawResponse(url, HttpMethod.Put, request, CancellationToken.None);
        }
        #endregion

        #region HTTP POST
        /// <summary>
        /// REST Post
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Post<T>([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetResponse<T>(url, HttpMethod.Post, request, token);
        }

        /// <summary>
        /// REST Post
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>        
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Post<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, HttpMethod.Post, request, CancellationToken.None);
        }

        /// <summary>
        /// REST Post (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Post([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, HttpMethod.Post, request, CancellationToken.None);
        }

        /// <summary>
        /// REST Post (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        protected Task<HttpResponseMessage> Post([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetRawResponse(url, HttpMethod.Post, request, token);
        }
        #endregion

        #region HTTP PATCH
        /// <summary>
        /// REST Patch
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Patch<T>([NotNull] string url, [CanBeNull] object request)
        {
            return GetResponse<T>(url, new HttpMethod("PATCH"), request, CancellationToken.None);
        }

        /// <summary>
        /// REST Patch
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Deserialized data of type T</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        protected Task<T> Patch<T>([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetResponse<T>(url, new HttpMethod("PATCH"), request, token);
        }

        /// <summary>
        /// REST Patch (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <returns>HttpResponseMessage</returns>
        protected Task<HttpResponseMessage> Patch([NotNull] string url, [CanBeNull] object request)
        {
            return GetRawResponse(url, new HttpMethod("PATCH"), request, CancellationToken.None);
        }

        /// <summary>
        /// REST Patch (RAW)
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="request">Request object (will be serialized to JSON if not string)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>HttpResponseMessage</returns>
        protected Task<HttpResponseMessage> Patch([NotNull] string url, [CanBeNull] object request, CancellationToken token)
        {
            return GetRawResponse(url, new HttpMethod("PATCH"), request, CancellationToken.None);
        }
        #endregion

        /// <summary>
        /// Override if you need custom HttpClientHandler
        /// </summary>
        /// <returns>HttpClientHandler</returns>
        protected virtual HttpBaseProtocolFilter CreateHttpFilter()
        {
            var handler = new HttpBaseProtocolFilter { AutomaticDecompression = true };
            return handler;
        }

        /// <summary>
        /// Creates a HTTP Client instance
        /// </summary>
        /// <param name="requestUrl">Request Url</param>
        /// <returns>HttpClient</returns>
        protected HttpClient CreateHttpClient([NotNull] string requestUrl)
        {
            var client = new HttpClient(_filter);
            var headers = GetRequestHeaders(requestUrl);
            foreach (var key in headers.Keys)
            {
                client.DefaultRequestHeaders.TryAppendWithoutValidation(key, headers[key]);
            }

            return client;
        }

        /// <summary>
        /// Creates JSON serializer settings. Can be overridden.
        /// </summary>
        /// <returns>JSON serializer settings</returns>
        protected virtual JsonSerializerSettings CreateJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }
        
        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        private Task GetResponse([NotNull] string url, [NotNull] HttpMethod method, [CanBeNull] object request, CancellationToken token)
        {
            return GetResponse<Object>(url, method, request, token, true);
        }

        /// <summary>
        /// Gets raw HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="noOutput">Output will not be proceed when true, method return default(T)</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>        
        private async Task<HttpResponseMessage> GetRawResponse([NotNull] string url, [NotNull] HttpMethod method, [CanBeNull] object request, CancellationToken token, bool noOutput = false)
        {
            await OnBeforeRequest(url, token).ConfigureAwait(false);

            HttpResponseMessage data = null;

            HttpStringContent requestcontent = null;

            string requestBody = null;

            var content = request as string;
            if (content != null)
            {
                requestcontent = new HttpStringContent(content);
            }
            else if (request != null)
            {
                requestBody = JsonConvert.SerializeObject(request, CreateJsonSerializerSettings());
                requestcontent = new HttpStringContent(requestBody, UnicodeEncoding.Utf8, "application/json");
            }

            try
            {
                var fullUrl = (new[] { "http://", "https://" }).Any(url.StartsWith) ? url : GetBaseUrl() + url;

                var client = CreateHttpClient(fullUrl);

                var requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(fullUrl),
                    Content = requestcontent,
                };

                Logger?.Info($"{method} {fullUrl}"+ (requestBody != null ? "\r\n"+ requestBody : ""));

                data = token == CancellationToken.None ? await client.SendRequestAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead) : await client.SendRequestAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).AsTask(token);                
                return data;
            }
            catch (TaskCanceledException)
            {
                Logger?.Error("Requesting {url} cancelled");
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error($"Error communicating with the server for {url}", ex);
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, data?.StatusCode ?? HttpStatusCode.ExpectationFailed, null);
            }
        }

        /// <summary>
        /// Gets HTTP response
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="request">HTTP request</param>
        /// <param name="token">Cancellation token</param>
        /// <param name="noOutput">Output will not be proceed when true, method return default(T)</param>
        /// <returns>Task</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        /// <exception cref="DeserializationException">When JSON parser cannot process the server response data</exception>
        private async Task<T> GetResponse<T>([NotNull] string url, [NotNull] HttpMethod method, [CanBeNull] object request, CancellationToken token, bool noOutput = false)
        {
            T result;
            var data = await GetRawResponse(url, method, request, token, noOutput);
            
            try
            {
                data.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                var content = await data.Content.ReadAsStringAsync();

                data.Content?.Dispose();

                Logger?.Error($"Error communicating with the server for {url}", ex);
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, data.StatusCode, content);
            }
            
            if (token != CancellationToken.None && token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            var json = await data.Content.ReadAsStringAsync();

            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Logger?.Error($"Error while processing response for {url}.", ex);
                throw new DeserializationException("Error while processing response. See the inner exception for details.", ex, json);
            }

            if (token != CancellationToken.None && token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            return result;
        }

        /// <summary>
        /// REST Head
        /// </summary>
        /// <param name="url">Url</param>        
        /// <returns>Dictionary with headers</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        public Task<Dictionary<string, string>> Head([NotNull] string url)
        {
            return Head(url, CancellationToken.None);
        }

        /// <summary>
        /// REST Head
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Dictionary with headers</returns>
        /// <exception cref="TaskCanceledException">When operation cancelled</exception>
        /// <exception cref="ConnectionException">When response from server does not indicate success</exception>
        public async Task<Dictionary<string, string>> Head([NotNull] string url, CancellationToken token)
        {
            await OnBeforeRequest(url, token).ConfigureAwait(false);

            try
            {
                Logger?.Info($"HEAD {GetBaseUrl() + url}");

                var client = CreateHttpClient(GetBaseUrl() + url);
                var request = new HttpRequestMessage(HttpMethod.Head, new Uri(GetBaseUrl() + url));
                var response = await client.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (token != CancellationToken.None && token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                return response.Headers.ToDictionary(headerItem => headerItem.Key, headerItem => headerItem.Value);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger?.Error($"Getting head for {url} failed", ex);
                throw new ConnectionException("Error communicating with the server. See the inner exception for details.", ex, HttpStatusCode.ExpectationFailed, null);
            }
        }
    }
}
