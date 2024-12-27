using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RMuseumBookReader.Pages
{
    public class IndexModel : PageModel
    {
        public async Task OnGetAsync(string book = "loc-m089-sehr-e-halal")
        {
            string url = $"{Configuration["APIRoot"]}/api/artifacts/limited/{book}/21";
            using (var client = new HttpClient())
            {

                using (var result = await client.GetAsync(url))
                {
                    if (result.IsSuccessStatusCode)
                    {
                        string json = await result.Content.ReadAsStringAsync();
                        var parsed = JObject.Parse(json);

                        BookName = parsed.SelectToken("name").Value<string>();
                        BookDescription = parsed.SelectToken("description").Value<string>().Replace("'", "").Replace("\"", "");
                        BookUrl = $"https://museum.ganjoor.net/items/{book}";
                        BookThumbnail = parsed.SelectToken("coverImage").SelectToken("externalNormalSizeImageUrl").Value<string>().Replace("/norm/", "/thumb/").Replace("/orig/", "/thumb/");

                        BookDataArray = "";

                        foreach (JToken image in parsed.SelectTokens("$.items[*].images[0]"))
                        {
                            BookDataArray += "[{";
                            BookDataArray += $"width:{image.SelectToken("normalSizeImageWidth").Value<string>()}, " +
                                $"height:{image.SelectToken("normalSizeImageHeight").Value<string>()}," +
                                $"uri:'{image.SelectToken("externalNormalSizeImageUrl").Value<string>()}'";
                            BookDataArray += "}],";
                        }

                        int imageCount = parsed.SelectToken("itemCount").Value<int>();
                        if (imageCount > 0)
                        {
                            JToken image = parsed.SelectTokens("$.items[*].images[0]").First();
                            for (int i = 21; i < imageCount; i++)
                            {
                                string imageUrl = book == "ai" ? $"https://i.ganjoor.net/images/{book}/orig/{$"{i}".PadLeft(7, '0')}.jpg" : $"https://i.ganjoor.net/images/{book}/norm/{$"{i}".PadLeft(4, '0')}.jpg";
                                BookDataArray += "[{";
                                BookDataArray += $"width:{image.SelectToken("normalSizeImageWidth").Value<string>()}, " +
                                    $"height:{image.SelectToken("normalSizeImageHeight").Value<string>()}," +
                                    $"uri:'{imageUrl}'";
                                BookDataArray += "}],";
                            }
                        }
                        

                        BookDataArray = $"[{BookDataArray}]";

                        ViewData["Title"] = BookName;
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

        public IConfiguration Configuration;

        public IndexModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
