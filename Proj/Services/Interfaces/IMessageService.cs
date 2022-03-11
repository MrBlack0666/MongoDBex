namespace mongoDB.Services
{
    /// <summary>
    /// Interfejs serwisu dotyczący wysyładnia wiadomości
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Metoda dotycząca wysyłania wiadomości email
        /// </summary>
        /// <param name="email">Email odbiorcy</param>
        /// <param name="message">Treść emaila</param>
        /// <returns></returns>
        bool SendEmail(string email, string message);
    }
}