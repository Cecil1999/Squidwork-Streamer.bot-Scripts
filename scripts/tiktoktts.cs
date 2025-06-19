using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.WebSockets;
using System.Media;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CPHInline
{
    public bool Execute()
    {
        var tts = args["rawInput"].ToString();
        string apiUrl = "https://tiktok-tts.weilnet.workers.dev/api/generation";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
        request.Method = "POST";
        request.ContentType = "application/json";

        var ttsOptions = new Dictionary<string, string>
        {
            // TEXT TTS will be speaking, DO NOT CHANGE THIS
            { "text", tts },

            // IF YOU WISH TO ADD MORE OPTIONS THE FORMAT IS
            // { "Tik Tok Option", "Value" },

            // Selected Voice, ONLY EDIT THE SECOND FIELD.
            { "voice", "en_us_001" },
        };
        string formData = JsonConvert.SerializeObject(ttsOptions);

        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        {
            writer.Write(formData);
        }

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        string base64Sound = reader.ReadToEnd();
        JObject jsonObject = JObject.Parse(base64Sound);
        byte[] soundBytes = Convert.FromBase64String((string)jsonObject["data"]);

        var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "temp.mp3");
        File.WriteAllBytes(fileName, soundBytes);

        CPH.LogDebug("TTS created");
        return true;
    }
}
