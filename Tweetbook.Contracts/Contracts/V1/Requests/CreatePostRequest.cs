﻿namespace Tweetbook.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }

        public List<CreateTagRequest> Tags { get; set; }
    }
}
