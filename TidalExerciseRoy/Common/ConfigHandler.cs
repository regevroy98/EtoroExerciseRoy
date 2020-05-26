using System.Configuration;

namespace TidalExerciseRoy.Common
{
    public class ConfigHandler
    {
        public static string Username => ConfigurationManager.AppSettings["userName"];

        public static string Password => ConfigurationManager.AppSettings["passWord"];

        public static int DefaultArtist => int.Parse(ConfigurationManager.AppSettings["defaultArtist"]);

        public static int MaxTrackToAddAtOnce => int.Parse(ConfigurationManager.AppSettings["maxTracksToAddAtOnce"]);

    }
}
