using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OpenTidl.Models;
using OpenTidl.Models.Base;
using TidalExerciseRoy.BL;
using TidalExerciseRoy.Common;

namespace TidalExerciseRoy.Test
{
    public class PlaylistAssertions : ReferenceTypeAssertions<CustomPlaylistModel, PlaylistAssertions>
    {

        private async Task<List<int>> RemoveIndicesFromExpectedTracks(RequestMaker requestMaker, int artistId, int[] removedIndices)
        {
            List<int> expectedTracksIds = await requestMaker.GetSongIds(artistId);


            List<int> indicesToRemove = removedIndices.ToList()
                .Select(indexToRemove => expectedTracksIds[indexToRemove]).ToList();

            indicesToRemove.ForEach(index => expectedTracksIds.Remove(index));

            return expectedTracksIds;

        }

        private async Task<int> GetTotalSongDurations (RequestMaker requestMaker, int artistId, int[] removedIndices)
        {
            JsonList<TrackModel> expectedTracks = await requestMaker.OpenTidlClient.GetArtistTopTracks(artistId, 0, ConfigHandler.MaxTrackToAddAtOnce);

            
            if (removedIndices != null)
            {

                List<TrackModel> tracksToRemove = removedIndices.ToList()
                    .Select(indexToRemove => expectedTracks.Items[indexToRemove]).ToList();

                List<TrackModel> tracks = expectedTracks.Items.ToList();
                tracksToRemove.ForEach(trackRemove => tracks.Remove(trackRemove));

                return tracks.Sum(track => track.Duration);
            }

            return expectedTracks.Items.Sum(track => track.Duration);

        }
        public PlaylistAssertions(CustomPlaylistModel modelToAssert)
        {
            Subject = modelToAssert;
        }
        public async Task<AndConstraint<CustomPlaylistModel>> HaveCorrectData(RequestMaker requestMaker, string playlistTitle, int artistId = 0, params int[] removedIndices)
        {

            artistId = artistId == 0 ? ConfigHandler.DefaultArtist : artistId;

            List<int> expectedTracksIds = removedIndices != null ?
                await RemoveIndicesFromExpectedTracks(requestMaker, artistId, removedIndices) :
                await requestMaker.GetSongIds(artistId);

            int expectedSongDuration = await GetTotalSongDurations(requestMaker, artistId, removedIndices);

            Subject.GetSongIds.Should().Contain(expectedTracksIds);
            Subject.Title.ShouldBeEquivalentTo(playlistTitle);
            Subject.TotalTrackDuration.ShouldBeEquivalentTo(expectedSongDuration);
            return new AndConstraint<CustomPlaylistModel>(Subject);
        }

        protected override string Context { get; }
    }

    public static class AssertionExtension
    {

        public static PlaylistAssertions Should(this CustomPlaylistModel playlistModel) => new PlaylistAssertions(playlistModel);
    }
}
