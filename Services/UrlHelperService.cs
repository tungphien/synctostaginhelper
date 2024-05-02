namespace SyncToStaging.Helper.Services
{
    public static class UrlHelperService
    {
        public const string PRINCIPLECODESERVICEURLCONSTANT = "principal";


        /// <summary>
        /// Swith sang BaseURL của internal, repace principal. sang empty
        /// </summary>
        /// <param name="initialUrl"></param>
        /// <returns></returns>
        public static string InternalBaseUrl(string initialUrl)
        {
            if (string.IsNullOrEmpty(initialUrl)) return default;
            return initialUrl.Replace($"{PRINCIPLECODESERVICEURLCONSTANT}.", string.Empty);
        }
        /// <summary>
        /// Swith sang BaseURL của external, repace principal sang code
        /// </summary>
        /// <param name="initialUrl"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ExternalBaseUrl(string initialUrl, string code)
        {
            if (string.IsNullOrEmpty(initialUrl) || string.IsNullOrEmpty(code))
            {
                return default;
            }
            return initialUrl.Replace(PRINCIPLECODESERVICEURLCONSTANT, code.ToLower());
        }

        /// <summary>
        /// Swith sang BaseURL của internal ở chế độ debug, bắt buộc phải có tham số code
        /// </summary>
        /// <param name="initialUrl"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string InternalBaseUrlDebug(string initialUrl, string code)
        {
            if (string.IsNullOrEmpty(initialUrl) || string.IsNullOrEmpty(code)) return default;
            return initialUrl.Replace(PRINCIPLECODESERVICEURLCONSTANT, code.ToLower());
        }
    }
}
