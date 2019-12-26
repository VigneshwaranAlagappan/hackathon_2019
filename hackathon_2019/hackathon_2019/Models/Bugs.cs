namespace hackathon_2019
{
    using System;

    public class Bugs
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Guid CreatedUserID
        {
            get;
            set;
        }

        public Guid ValidatedUserID
        {
            get;
            set;
        }

        public Platform Platform
        {
            get;
            set;
        }

        public int Severity
        {
            get;
            set;
        }
    }
}