using System.Collections.Generic;

namespace BecauseWeCare.Web.Models
{
    public class Forum
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Topic
    {
        public int id { get; set; }
        public string prompt { get; set; }
        public string example { get; set; }
        public int votes_allowed { get; set; }
        public int suggestions_count { get; set; }
        public int open_suggestions_count { get; set; }
        public bool closed { get; set; }
        public bool anonymous_access { get; set; }
        public bool unlimited_votes { get; set; }
        public object closed_at { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Forum forum { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Creator
    {
        public int id { get; set; }
        public string name { get; set; }
        public object title { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
        public int karma_score { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Creator2
    {
        public int id { get; set; }
        public string name { get; set; }
        public object title { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
        public int karma_score { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Response
    {
        public string text { get; set; }
        public string formatted_text { get; set; }
        public string created_at { get; set; }
        public Creator2 creator { get; set; }
    }

    public class Status
    {
        public int id { get; set; }
        public string name { get; set; }
        public string hex_color { get; set; }
        public string key { get; set; }
    }

    public class StatusChangedBy
    {
        public int id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string avatar_url { get; set; }
        public int karma_score { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Site
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Subdomain { get; set; }
    }

    public class Suggestion
    {
        public string url { get; set; }
        public int id { get; set; }
        public string state { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string formatted_text { get; set; }
        public object referrer { get; set; }
        public int vote_count { get; set; }
        public int subscriber_count { get; set; }
        public int comments_count { get; set; }
        public int supporters_count { get; set; }
        public Topic topic { get; set; }
        public Category category { get; set; }
        public object closed_at { get; set; }
        public Status status { get; set; }
        public StatusChangedBy status_changed_by { get; set; }
        public Creator creator { get; set; }
        public Response response { get; set; }
        public List<object> attachments { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Site Site { get; set; }
    }
}
