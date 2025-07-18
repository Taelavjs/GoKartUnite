﻿using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.HelpersTut
{
    public class HtmlHelpers
    {
        public static async Task<IHtmlDocument> GetDocumentAsyncaa(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var document = await BrowsingContext.New()
                .OpenAsync(ResponseFactory, CancellationToken.None);
            return (IHtmlDocument)document;

            void ResponseFactory(VirtualResponse htmlResponse)
            {
                htmlResponse
                    .Address(response.RequestMessage.RequestUri)
                    .Status(response.StatusCode);

                MapHeaders(response.Headers);
                MapHeaders(response.Content.Headers);

                htmlResponse.Content(content);

                void MapHeaders(HttpHeaders headers)
                {
                    foreach (var header in headers)
                    {
                        foreach (var value in header.Value)
                        {
                            htmlResponse.Header(header.Key, value);
                        }
                    }
                }
            }
        }
    }
}
