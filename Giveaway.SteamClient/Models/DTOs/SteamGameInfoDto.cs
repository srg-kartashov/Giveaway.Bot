using Newtonsoft.Json;

namespace Giveaway.Steam.Models.DTOs
{
    internal class SteamGameInfoDto
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("query_summary")]
        public QuerySummaryDto QuerySummary { get; set; }

        [JsonProperty("reviews")]
        public List<ReviewDto> Reviews { get; set; }

        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }

    internal class AuthorDto
    {
        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("num_games_owned")]
        public int NumGamesOwned { get; set; }

        [JsonProperty("num_reviews")]
        public int NumReviews { get; set; }

        [JsonProperty("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonProperty("playtime_last_two_weeks")]
        public int PlaytimeLastTwoWeeks { get; set; }

        [JsonProperty("playtime_at_review")]
        public int PlaytimeAtReview { get; set; }

        [JsonProperty("last_played")]
        public int LastPlayed { get; set; }
    }

    internal class QuerySummaryDto
    {
        [JsonProperty("num_reviews")]
        public int NumReviews { get; set; }

        [JsonProperty("review_score")]
        public int ReviewScore { get; set; }

        [JsonProperty("review_score_desc")]
        public string ReviewScoreDesc { get; set; }

        [JsonProperty("total_positive")]
        public int TotalPositive { get; set; }

        [JsonProperty("total_negative")]
        public int TotalNegative { get; set; }

        [JsonProperty("total_reviews")]
        public int TotalReviews { get; set; }
    }

    internal class ReviewDto
    {
        [JsonProperty("recommendationid")]
        public string Recommendationid { get; set; }

        [JsonProperty("author")]
        public AuthorDto Author { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("review")]
        public string ReviewData { get; set; }

        [JsonProperty("timestamp_created")]
        public int TimestampCreated { get; set; }

        [JsonProperty("timestamp_updated")]
        public int TimestampUpdated { get; set; }

        [JsonProperty("voted_up")]
        public bool VotedUp { get; set; }

        [JsonProperty("votes_up")]
        public int VotesUp { get; set; }

        [JsonProperty("votes_funny")]
        public int VotesFunny { get; set; }

        [JsonProperty("weighted_vote_score")]
        public string WeightedVoteScore { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("steam_purchase")]
        public bool SteamPurchase { get; set; }

        [JsonProperty("received_for_free")]
        public bool ReceivedForFree { get; set; }

        [JsonProperty("written_during_early_access")]
        public bool WrittenDuringEarlyAccess { get; set; }

        [JsonProperty("hidden_in_steam_china")]
        public bool HiddenInSteamChina { get; set; }

        [JsonProperty("steam_china_location")]
        public string SteamChinaLocation { get; set; }
    }
}
