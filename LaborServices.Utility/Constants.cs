namespace LaborServices.Utility
{
    #region Operational Enums

    public enum Language
    {
        English = 0,
        Arabic = 1
    }

    public enum CustomerType
    {
        Individaul = 0,
        Business = 1
    }

    public enum DayShifts
    {
        Morning = 0,
        Evening = 1
    }

    public enum WebSitePageNames : byte
    {
        ContactUs = 0,
        About = 1
    }

    public enum SectorsTypeEnum : byte
    {
        Business = 1,
        Individuals = 2,
        HeadOffice = 3
    }

    /// <summary>
    /// complaints problem Types
    /// </summary>
    public enum ProblemTypes
    {
        Others = 100000005,
        CancellationRequest = 100000008,
        ComplainOnEmployee = 100000009,
    }

    public enum UserAccountType
    {
        Admin = 0
    }

    /// <summary>
    /// Data Types : Integer, Fraction ,
    /// SingleText,MultiText,
    ///  Date,Password,
    ///  Boolean
    /// </summary>
    public enum DataTypes : byte
    {
        /// <summary>
        /// Int, decimal
        /// </summary>
        Integer = 1,

        /// <summary>
        /// double, float 
        /// </summary>
        Fraction = 2,

        /// <summary>
        /// Single Text
        /// </summary>
        SingleText = 3,

        /// <summary>
        /// Multi Text
        /// </summary>
        MultiText = 4,

        /// <summary>
        /// Date Time 
        /// </summary>
        Date = 5,

        /// <summary>
        /// passwords 
        /// </summary>
        Password = 6,

        /// <summary>
        /// boolean
        /// </summary>
        Boolean = 7
    }

    public enum TransactionType
    {
        ServiceContractPerHour =1,
        DomesticInvoice = 2
    }

    #endregion

    public class AppConstants
    {
        public const string DefaultLang = "ar";
        public const string DefaultUserName = "0555555555";
        public const string DefaultUserEmail = "laborAdmin@Innovations.com";
        public const string DefaultPass = "Admin@123456";
        public const string AdminRoleName = "Admin";
        public const string MobilePhone = "0555555555";
        public const string SuperAdminsGroup = "SuperAdmins";
        //public const string MobilePhoneRex = @"^(009665|9665|\+9665|05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$";
        public const string MobilePhoneRex = @"^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$";
        public const string IdNumberRex = @"^([0-9]{10})$";

        public const int Who_WebSource = 2;

        #region menu Names
        #region  Agent
        public const string AgentIndexAr = "العملاء";
        public const string AgentIndexEn = "Agents";

        public const string AgentCreateAr = "إنشاء عميل جديد";
        public const string AgentCreateEn = "Create new Agent";

        #endregion
        #endregion


        #region ImagesPath

        public const string SliderFolder = "~/SliderImages/";

		public const string aboutFolder = "~/Images/uploads/about/";

		#endregion
	}
}
