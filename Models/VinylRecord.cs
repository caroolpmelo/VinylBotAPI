using Google.Cloud.Firestore;

namespace VinylBot.Api.Models
{
    [FirestoreData]
    public class VinylRecord
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("artist")]
        public string Artist { get; set; } = string.Empty;

        [FirestoreProperty("album")]
        public string Album { get; set; } = string.Empty;

        [FirestoreProperty("year")]
        public int Year { get; set; }

        [FirestoreProperty("genre")]
        public string Genre { get; set; } = string.Empty;

        [FirestoreProperty("coverColor")]
        public string CoverColor { get; set; } = string.Empty;

        [FirestoreProperty("trackList")]
        public List<string> TrackList { get; set; } = new List<string>();

        [FirestoreProperty("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
