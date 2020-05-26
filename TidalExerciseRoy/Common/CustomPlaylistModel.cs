using System;
using System.Collections.Generic;
using System.Linq;
using OpenTidl.Models;

namespace TidalExerciseRoy.Common
{
    public class CustomPlaylistModel
    {
        public CustomPlaylistModel(String eTag, String title, String uuid, CreatorModel creatorModel, TrackModel [] trackModels = null)
        {
            ETag = eTag;
            Title = title;
            Uuid = uuid;
            CreatorModel = creatorModel;
            TrackModels = trackModels;
        }


        public string Title { get; }

        public string ETag { get; set; }

        public int TotalTrackDuration => TrackModels.ToList().Sum(track => track.Duration);
        public TrackModel[] TrackModels { get; set; }

        public string Uuid { get;  }

        public CreatorModel CreatorModel { get; }

        public List<int> GetSongIds => TrackModels.Select(track => track.Id).ToList();

    }

}
