using System.Threading.Tasks;
using NUnit.Framework;
using TidalExerciseRoy.Common;

namespace TidalExerciseRoy.Test
{
    public class Tests : TestBase
    {

        [Test]
        [Description("JIRA-ISSUE-1")]
        [Author("RoyReg")]
        public async Task Playlist_WhenCreatingNew_ShouldHaveCorrectData()
        {

            CustomPlaylistModel playlist = await RequestMaker.CreatePlaylist(RandomId);
            playlist = await playlist.AddSongsToPlaylist(RequestMaker);

            await playlist.Should().HaveCorrectData(RequestMaker, RandomId);
        }

        [Test]
        [Description("JIRA-ISSUE-2")]
        [Author("RoyReg")]
        public async Task Playlist_WhenUpdatingName_ShouldSaveChanges()
        {

            string newPlaylistName = "BATMAN!";
            CustomPlaylistModel playlist = await RequestMaker.CreatePlaylist(RandomId);
            playlist = await playlist.AddSongsToPlaylist(RequestMaker);

            playlist = await playlist.UpdatePlaylistName(RequestMaker, newPlaylistName);
            await playlist.Should().HaveCorrectData(RequestMaker, newPlaylistName);
        }


        [Test]
        [Description("JIRA-ISSUE-3")]
        [Author("RoyReg")]
        public async Task Playlist_WhenDeletingTracks_ShouldSaveChanges()
        {
            CustomPlaylistModel playlist = await RequestMaker.CreatePlaylist(RandomId);

            playlist = await playlist.AddSongsToPlaylist(RequestMaker);


            playlist = await playlist.DeleteTracksFromPlaylist(RequestMaker, 0, 1, 2);


            await playlist.Should().HaveCorrectData(RequestMaker, RandomId, 0, 0, 1, 2);
        }
    }
}