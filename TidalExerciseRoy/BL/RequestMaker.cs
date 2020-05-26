using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTidl;
using OpenTidl.Methods;
using OpenTidl.Models;
using OpenTidl.Models.Base;
using OpenTidl.Transport;
using TidalExerciseRoy.Common;
using static NLog.LogManager;

namespace TidalExerciseRoy.BL
{
    public class RequestMaker
    {
        private static readonly NLog.Logger Logger = GetCurrentClassLogger();

        public readonly OpenTidlClient OpenTidlClient;
        public OpenTidlSession OpenTidlSession;
        private readonly ClientConfiguration _defaultClientConfiguration = ClientConfiguration.Default;
        public PlaylistModel PlaylistModel;
        public RequestMaker()
        {
            try
            {
                Logger.Info("Attempting to create new Tidl client");
                Logger.Warn("Make sure the client configuration is correct!");
                OpenTidlClient = new OpenTidlClient(config: new ClientConfiguration(_defaultClientConfiguration.ApiEndpoint,
                    _defaultClientConfiguration.UserAgent, "pl4Vc0hemlAXD0mN", _defaultClientConfiguration.ClientUniqueKey,
                    _defaultClientConfiguration.ClientVersion, _defaultClientConfiguration.DefaultCountryCode));

            }
            catch (OpenTidlException e)
            {
                Logger.Error(e, "Failed to instantiate new tidl client due to exception");
                Console.WriteLine(e);
                throw;


            }

            Logger.Info("Successfully created new Tidl client with token {0}", OpenTidlClient.Configuration.Token);



        }
        public async Task Login(string username, string password)
        {
            try
            {
                OpenTidlSession = await OpenTidlClient.LoginWithUsername(
                    username, password);
            }
            catch (OpenTidlException e)
            {
                Logger.Error(e, "Failed to log in with user {0} and password {1}", username, password);
                Console.WriteLine(e);
                throw;
            }


            Logger.Info("Successfully logged in with user {0}!", username);


        }

        public async Task<CustomPlaylistModel> CreatePlaylist(string name)
        {
            Logger.Info("Creating new playlist!");
            try
            {
                PlaylistModel = await OpenTidlSession.CreateUserPlaylist(name);

            }
            catch (Exception e)
            {
                Logger.Error("Could not create playlist due to internal error");
                Console.WriteLine(e);
                throw;
            }
            return new CustomPlaylistModel(PlaylistModel.ETag, PlaylistModel.Title, PlaylistModel.Uuid, PlaylistModel.Creator);
        }


        public async Task<CustomPlaylistModel> FetchUpdatedPlaylistModel(string uuid)
        {
            try
            {
                Logger.Info("Attempting to fetch updated playlist");
                PlaylistModel = await OpenTidlSession.GetPlaylist(uuid);
                var playlistTracks = await OpenTidlSession.GetPlaylistTracks(PlaylistModel.Uuid);
                return new CustomPlaylistModel(PlaylistModel.ETag, PlaylistModel.Title, PlaylistModel.Uuid,
                    PlaylistModel.Creator, playlistTracks.Items);
            }

            catch (OpenTidlException e)
            {
                Logger.Error(e, "Was not able to fetch updated playlist");
                Console.WriteLine(e);
                throw;
            }
        }


        public async Task<List<int>> GetSongIds(int artistId = 0)
        {
            artistId = artistId == 0 ? ConfigHandler.DefaultArtist : artistId;
            try
            {
                Logger.Info("Attempting to get top tracks for artist {0}", artistId);

                JsonList<TrackModel> artistTopTracks = await OpenTidlClient.GetArtistTopTracks(artistId, 0, ConfigHandler.MaxTrackToAddAtOnce);

                List<int> songIds = artistTopTracks.Items
                    .Select(song => song.Id).ToList();

                return songIds;
            }
            catch (OpenTidlException e)
            {
                Logger.Error(e, "Couldn't find top tracks for artist");
                Console.WriteLine(e);
                throw;
            }


        }

    }
}
;