using System.ComponentModel;

namespace Bounty.DataClasses
{
    public enum Platform
    {
        BoldBI = 1,
        BoldReports,
        DataIntegrationPlatform,
        XamarinForms
    }

    public enum UserRole
    {
        Management = 1,
        PlatformManager,
        Engineer
    }

    public enum AggregateMethod
    {
        /// <summary>
        ///     Aggregation will not be applied
        /// </summary>
        None,

        /// <summary>
        ///     Returns the number of rows
        /// </summary>
        COUNT,

        /// <summary>
        ///     Returns the Maximum value in the given column
        /// </summary>
        MAX,

        /// <summary>
        ///     Returns the Minimum value in the given column
        /// </summary>
        MIN,

        /// <summary>
        ///     Returns the Average of the given column
        /// </summary>
        AVG,

        /// <summary>
        ///     Returns the SUM of the given column
        /// </summary>
        SUM,

        /// <summary>
        ///     Returns the Standard deviation of the given column
        /// </summary>
        STDEV,

        /// <summary>
        ///     Returns the variance of all the values in the given column
        /// </summary>
        VAR
    }

    /// <summary>
    ///     SQL Conditions
    /// </summary>
    public enum Condition
    {
        /// <summary>
        ///     No Condition will be applied
        /// </summary>
        None,

        /// <summary>
        ///     Applies Equal Operator
        /// </summary>
        Equals,

        /// <summary>
        ///     Applies Not Equal Operator
        /// </summary>
        NotEquals,

        /// <summary>
        ///     Applies Lesser than Operator
        /// </summary>
        LessThan,

        /// <summary>
        ///     Applies Greater than Operator
        /// </summary>
        GreaterThan,

        /// <summary>
        ///     Applies Lesser than or Equal Operator
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        ///     Applies Greater than or Equals Operator
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        IS,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        IN,

        /// <summary>
        ///     Applies for NULL values
        /// </summary>
        LIKE,

        NOTIN
    }

    /// <summary>
    ///     SQL Logical Operators
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        ///     No Condition Will be applied
        /// </summary>
        None,

        /// <summary>
        ///     Applies Logical OR operation
        /// </summary>
        OR,

        /// <summary>
        ///     Applies Logical AND operation
        /// </summary>
        AND,

        /// <summary>
        ///     Applies Logical IN operation
        /// </summary>
        IN,

        /// <summary>
        ///     Applies Logical LIKE operation
        /// </summary>
        LIKE,

        /// <summary>
        ///     Applies Logical NOT operation
        /// </summary>
        NOT
    }

    public enum DirectoryType
    {
        [Description("All Users")]
        All = 0,

        [Description("Local User Account")]
        Local = 1,

        [Description("Azure Active Directory Account User")]
        AzureAD,

        [Description("External Database User")]
        ExternalDatabase,

        [Description("GSuite User")]
        GSuite,

        [Description("Syncfusion User")]
        Syncfusion
    }
}