using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RoundedWindowsEdges
{
    public static class Updater
    {
        public static async Task CheckForUpdatesAsync(string currentVersion)
        {
            try
            {
                string updateUrl = "https://api.github.com/repos/mauriciobellon/rounded-windows-edges/releases/latest";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RoundedWindowsEdgesUpdater");
                    var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    }
                    var response = await client.GetAsync(updateUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var release = JsonSerializer.Deserialize<GitHubRelease>(json);
                        if (release != null && IsNewerVersion(release.tag_name, currentVersion))
                        {
                            MessageBox.Show($"A new version ({release.tag_name}) is available. Please visit {release.html_url} to download the latest version.", "Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        }

        private static bool IsNewerVersion(string latestVersion, string currentVersion)
        {
            if (Version.TryParse(latestVersion, out var vLatest) && Version.TryParse(currentVersion, out var vCurrent))
            {
                return vLatest > vCurrent;
            }
            return false;
        }
    }

    public class UpdateInfo
    {
        public string LatestVersion { get; set; }
        public string DownloadUrl { get; set; }
    }
}
