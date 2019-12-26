namespace Bounty.DataClasses
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

        public bool IsActive
        {
            get;
            set;
        }

        public bool IsDeleted
        {
            get;
            set;
        }
    }
}