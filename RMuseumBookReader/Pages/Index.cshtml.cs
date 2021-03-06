﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RMuseumBookReader.Pages
{
    public class IndexModel : PageModel
    {
        public async Task OnGetAsync(string book = "loc-m089-sehr-e-halal")
        {
            string url = $"https://ganjgah.ir/api/artifacts/{book}";
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        string json = await result.Content.ReadAsStringAsync();
                        var parsed = JObject.Parse(json);

                        BookName = parsed.SelectToken("name").Value<string>();
                        BookDescription = parsed.SelectToken("description").Value<string>();
                        BookUrl = $"https://museum.ganjoor.net/items/{book}";
                        BookThumbnail = $"https://ganjgah.ir/api/images/norm/{parsed.SelectToken("coverImageId").Value<string>()}.jpg";

                        BookDataArray = "";

                        foreach (JToken image in parsed.SelectTokens("$.items[*].images[0]"))
                        {
                            BookDataArray += "[{";
                            BookDataArray += $"width:{image.SelectToken("normalSizeImageWidth").Value<string>()}, " +
                                $"height:{image.SelectToken("normalSizeImageHeight").Value<string>()}," +
                                $"uri:'https://ganjgah.ir/api/images/norm/{image.SelectToken("id").Value<string>()}.jpg'";
                            BookDataArray += "}],";
                        }

                        BookDataArray = $"[{BookDataArray}]";
                    }
                    else
                    {
                       
                    }
                }
            }
        }

        /// <summary>
        /// Book images
        /// </summary>
        public string BookDataArray { get; set; }

        /// <summary>
        /// Book Name
        /// </summary>
        public string BookName { get; set; }

        /// <summary>
        /// Book Description
        /// </summary>
        public string BookDescription { get; set; }

        /// <summary>
        /// Book Url
        /// </summary>
        public string BookUrl { get; set; }

        /// <summary>
        /// Book Thumbnail
        /// </summary>
        public string BookThumbnail { get; set; }
    }
}
