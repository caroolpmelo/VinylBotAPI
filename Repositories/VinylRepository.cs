using Google.Cloud.Firestore;
using VinylBot.Api.Models;

namespace VinylBot.Api.Repositories
{
    public class VinylRepository
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly CollectionReference _collection;

        public VinylRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _collection = _firestoreDb.Collection("vinylRecords");
        }

        public async Task<List<VinylRecord>> GetAllRecordsAsync()
        {
            var snapshot = await _collection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<VinylRecord>()).ToList();
        }

        public async Task<VinylRecord?> GetRecordByIdAsync(string id)
        {
            var docRef = _collection.Document(id.Trim().ToLowerInvariant());
            var snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<VinylRecord>() : null;
        }

        public async Task<string> AddRecordAsync(VinylRecord record)
        {
            record.Id = Guid.NewGuid().ToString("N")[..6];
            var docRef = _collection.Document(record.Id);
            await docRef.SetAsync(record);
            return record.Id;
        }

        public async Task<bool> UpdateRecordAsync(string id, VinylRecord updatedRecord)
        {
            var docRef = _collection.Document(id.Trim().ToLowerInvariant());
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false;
            }

            await docRef.SetAsync(updatedRecord, SetOptions.Overwrite);
            return true;
        }

        public async Task<bool> DeleteRecordAsync(string id)
        {
            var docRef = _collection.Document(id.Trim().ToLowerInvariant());
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                return false;
            }

            await docRef.DeleteAsync();
            return true;
        }

        public async Task<List<VinylRecord>> SearchRecordsByTermAsync(string query)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var normalizedQuery = query.Trim().ToLowerInvariant();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.Artist.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                                 record.Album.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                                 record.TrackList.Any(track => track.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(record => record.Artist)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByArtistAsync(string artist)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var normalizedArtist = artist.Trim().ToLowerInvariant();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.Artist.Contains(normalizedArtist, StringComparison.OrdinalIgnoreCase))
                .OrderBy(record => record.Album)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByYearAsync(int year)
        {
            var snapshot = await _collection.GetSnapshotAsync();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.Year == year)
                .OrderBy(record => record.Artist)
                .ThenBy(record => record.Album)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByGenreAsync(string genre)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var normalizedGenre = genre.Trim().ToLowerInvariant();

            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.Genre.Contains(normalizedGenre, StringComparison.OrdinalIgnoreCase))
                .OrderBy(record => record.Artist)
                .ThenBy(record => record.Album)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByCoverColorAsync(string coverColor)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var normalizedCoverColor = coverColor.Trim().ToLowerInvariant();
            
            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.CoverColor.Contains(normalizedCoverColor, StringComparison.OrdinalIgnoreCase))
                .OrderBy(record => record.Artist)
                .ThenBy(record => record.Album)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByTrackAsync(string track)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var normalizedTrack = track.Trim().ToLowerInvariant();
            
            return snapshot.Documents
                .Select(doc => doc.ConvertTo<VinylRecord>())
                .Where(record => record.TrackList.Any(t => t.Contains(normalizedTrack, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(record => record.Artist)
                .ThenBy(record => record.Album)
                .ToList();
        }

        public async Task<List<VinylRecord>> GetRecordsByMultipleCriteriaAsync(string? artist = null, int? year = null, string? genre = null, string? coverColor = null, string? track = null)
        {
            var snapshot = await _collection.GetSnapshotAsync();
            var records = snapshot.Documents.Select(doc => doc.ConvertTo<VinylRecord>()).AsQueryable();
            if (!string.IsNullOrWhiteSpace(artist))
            {
                var normalizedArtist = artist.Trim().ToLowerInvariant();
                records = records.Where(record => record.Artist.Contains(normalizedArtist, StringComparison.OrdinalIgnoreCase));
            }
            if (year.HasValue)
            {
                records = records.Where(record => record.Year == year.Value);
            }
            if (!string.IsNullOrWhiteSpace(genre))
            {
                var normalizedGenre = genre.Trim().ToLowerInvariant();
                records = records.Where(record => record.Genre.Contains(normalizedGenre, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(coverColor))
            {
                var normalizedCoverColor = coverColor.Trim().ToLowerInvariant();
                records = records.Where(record => record.CoverColor.Contains(normalizedCoverColor, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(track))
            {
                var normalizedTrack = track.Trim().ToLowerInvariant();
                records = records.Where(record => record.TrackList.Any(t => t.Contains(normalizedTrack, StringComparison.OrdinalIgnoreCase)));
            }
            return records.OrderBy(record => record.Artist).ThenBy(record => record.Album).ToList();
        }
    }
}
