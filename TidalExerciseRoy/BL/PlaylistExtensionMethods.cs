using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTidl.Models;
using OpenTidl.Models.Base;
using OpenTidl.Transport;
using TidalExerciseRoy.BL;
using static NLog.LogManager;

namespace TidalExerciseRoy.Common
{
    public static class PlaylistExtensionMethods
    {
        private static readonly NLog.Logger Logger = GetCurrentClassLogger();

        public static async Task<CustomPlaylistModel> AddSongsToPlaylist(this CustomPlaylistModel playlist, RequestMaker requestMaker, int artistId = 0)
        {
            artistId = artistId == 0 ? ConfigHandler.DefaultArtist : artistId;
            try
            {
                Logger.Info("Attempting to add songs to playlist");
                List<int> songsToAdd = await requestMaker.GetSongIds(artistId);
                await requestMaker.OpenTidlSession.AddPlaylistTracks(playlist.Uuid, playlist.ETag, songsToAdd);
                playlist = await requestMaker.FetchUpdatedPlaylistModel(playlist.Uuid);
                JsonList<TrackModel> playlistTrack =
                    await requestMaker.OpenTidlSession.GetPlaylistTracks(playlist.Uuid);
                playlist.TrackModels = playlistTrack.Items;

                return await requestMaker.FetchUpdatedPlaylistModel(playlist.Uuid);
            }

            catch (OpenTidlException e)
            {
                Logger.Error(e, "Could not add track ids to playlist {0}", playlist.Uuid);
                throw;
            }
        }



        public static async Task<CustomPlaylistModel> FetchUpdatedPlaylist(this CustomPlaylistModel playlistModel, RequestMaker requestMaker)
        {

            return await requestMaker.FetchUpdatedPlaylistModel(playlistModel.Uuid);

        }

        public static async Task<CustomPlaylistModel> DeleteTracksFromPlaylist(this CustomPlaylistModel playlistModel,
            RequestMaker requestMaker, params int[] indices)
        {

            try
            {
                Logger.Info("Attempting to delete tracks from playlist");
                await requestMaker.OpenTidlSession.DeletePlaylistTracks(playlistModel.Uuid, playlistModel.ETag,
                    indices);
                return await requestMaker.FetchUpdatedPlaylistModel(playlistModel.Uuid);
            }

            catch (OpenTidlException e)
            {
                Logger.Error(e, "Could not add track ids to playlist {0}", playlistModel.Uuid);
                throw;

            }
        }

        public static async Task<CustomPlaylistModel> UpdatePlaylistName(this CustomPlaylistModel playlistModel,
            RequestMaker requestMaker, string newTitle)
        {

            try
            {
                Logger.Info("Attempting to update playlist name from {0} to {1}", playlistModel.Title, newTitle);
                await requestMaker.OpenTidlSession.UpdatePlaylist(playlistModel.Uuid, playlistModel.ETag, newTitle);
                return await requestMaker.FetchUpdatedPlaylistModel(playlistModel.Uuid);

            }
            catch (OpenTidlException e)
            {
                Logger.Error(e, "Could not update playlist name for playlist {0}", playlistModel.Uuid);

                throw;
            }
        }

    }
}