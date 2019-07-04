namespace Fanex.NAP.SPU.Core
{
    public static class Constants
    {
        public const string VirtualSettingXmlRoot = "<ROOT>{0}</ROOT>";

        public const string VirtualSettingXmlForUpdating = "<Rate CustIds=\"{0}\" VirtualMultiplier =\"{1} \" />";

        public const string VirtualSettingXmlForLogging = "<Rate CustID=\"{0}\" ReportDate=\"{1}\" SiteID=\"{2}\" SRecommend=\"{3}\"" +
                                                        " MRecommend=\"{4}\" Recommend=\"{5}\" OldVirtualRate=\"{6}\" NewVirtualRate =\"{7}\" IsBA =\"{8}\" />";

        public const string VirtualSettingXmlForCustomerMarginHistory = "<Rate CustID=\"{0}\" ReportMonth=\"{1}\" ReportYear=\"{2}\" Turnover=\"{3}\"" +
                                                        " Winloss=\"{4}\" BetCount=\"{5}\" ServiceName=\"{6}\" />";


    }
}