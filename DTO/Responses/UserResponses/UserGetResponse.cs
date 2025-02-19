﻿namespace SOFTITOFLIX.DTO.Responses
{
    public class UserGetResponse
    {
        public long Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public bool Passive { get; set; }
    }
}
