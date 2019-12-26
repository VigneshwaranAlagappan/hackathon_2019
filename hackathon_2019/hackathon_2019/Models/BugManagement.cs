namespace hackathon_2019
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    public class BugManagement
    {
        public void UpdateBug(Bugs bug)
        {
            var values = new Dictionary<string, object>
                    {
                        { "Description", bug.Description },
                        { "Title", bug.Title },
                        { "PlatformId", bug.Platform }
                    };

            var whereColumns = new List<ConditionColumn>
                {
                    new ConditionColumn
                    {
                        ColumnName = "Id",
                        Condition = Condition.Equals,
                        Value = bug.Id
                    },
                    new ConditionColumn
                    {
                        ColumnName = "Id",
                        Condition = Condition.Equals,
                        Value = bug.Id,
                        LogicalOperator = LogicalOperator.AND
                    }
                };

            var s = QueryBuilder.UpdateRowInTable("Bug", values, whereColumns);
        }

        public void AddBug(Bugs bug)
        {
            var bugResult = new Result();
            bug.Id = Guid.NewGuid();
            var values = new Dictionary<string, object>
                    {
                        { "Id", bug.Id },
                        { "Description", bug.Description },
                        { "Title", bug.Title },
                        { "PlatformId", bug.Platform },
                        { "CreatedUserId", bug.CreatedUserID },                      
                        { "IsActive", true }
                    };

            bugResult = DataProvider.ExecuteNonQuery(QueryBuilder.AddToTable("Bug",values), Connection.ConnectionString);

            if (!bugResult.Status)
            {
                //exception
            }
        }
        public Users AddUser(Users user)
        {
            try
            {
                if (!IsExistingEmail(user.Email))
                {
                    // Create new user Id
                    user.Id = Guid.NewGuid();
                    user.DisplayName = new MailAddress(user.Email).User;
                    var values = new Dictionary<string, object>
                    {
                        { "Id", user.Id },
                        { "DisplayName", user.DisplayName },
                        { "Email", user.Email },
                        { "PlatformId", user.Platform },
                        { "UserRole", user.Role },
                        { "IsActive", true },
                        { "IsDeleted", false }
                    };

                    var userResult = new Result();

                    userResult = DataProvider.ExecuteNonQuery(QueryBuilder.AddToTable("User", values), Connection.ConnectionString);

                    if (!userResult.Status)
                    {
                        //exception
                    }

                    return user;
                }
            }
            catch (Exception e)
            {
               
            }

            return null;
        }

        public bool IsExistingEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                var result = DataProvider.ExecuteReaderQuery(QueryBuilder.IsExistingEmailQuery(email));

                result.Status = result.DataTable.Rows.Count > 0;

                return result.Status;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}