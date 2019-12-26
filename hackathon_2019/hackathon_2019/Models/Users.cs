﻿namespace hackathon_2019
{
    using System;

    public class Users
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public UserRole Role
        {
            get;
            set;
        }

        public Platform Platform
        {
            get;
            set;
        }
    }
}