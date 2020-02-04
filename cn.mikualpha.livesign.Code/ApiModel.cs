using Native.Csharp.Sdk.Cqp;

namespace cn.mikualpha.livesign.Code
{
    class ApiModel
    {
        public static CQApi CQApi { get; private set; }
        public static CQLog CQLog { get; private set; }

        public static void setModel(CQApi _API, CQLog _LOG)
        {
            CQApi = _API;
            CQLog = _LOG;
        }
    }
}
