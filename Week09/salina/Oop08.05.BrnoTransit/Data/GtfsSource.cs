namespace BrnoTransit.Data
{
    // downloads the timetable of IDS JMK and keeps a copy on disk for a week
    // open data of the city of Brno, licence CC BY 4.0: https://data.brno.cz
    public static class GtfsSource
    {
        public const string Url =
            "https://www.arcgis.com/sharing/rest/content/items/379d2e9a7907460c8ca7fda1f3e84328/data";

        public static string GetZip(string folder)
        {
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "idsjmk_gtfs.zip");
            if (File.Exists(path) && File.GetLastWriteTime(path) > DateTime.Now.AddDays(-7))
            {
                Console.WriteLine("timetable: the copy from " + File.GetLastWriteTime(path).ToString("d.M.yyyy HH:mm"));
                return path;
            }

            Console.WriteLine("timetable: downloading from IDS JMK open data, this takes a while...");
            try
            {
                Download(path);
            }
            catch (Exception error)
            {
                if (!File.Exists(path))
                {
                    throw;
                }
                Console.WriteLine("timetable: download failed (" + error.Message + "), using the old copy");
            }
            return path;
        }

        private static void Download(string path)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(10);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
                HttpResponseMessage response = client.Send(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                using (Stream web = response.Content.ReadAsStream())
                using (FileStream file = File.Create(path + ".part"))
                {
                    web.CopyTo(file);
                }
            }
            File.Move(path + ".part", path, true);
        }
    }
}
