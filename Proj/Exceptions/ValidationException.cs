using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Exceptions
{
    /// <summary>
    /// Wyjątek mówiący o błędach walidacyjnych przy dodawaniu do bazy mongo
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Tytuł wyjątku.
        /// </summary>
        public string Title { get; }
        /// <summary>
        /// Wiadomość wyjątku
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// Konstruktor tworzący wyjątek
        /// </summary>
        /// <param name="title">Tytuł wyjątku</param>
        /// <param name="message">Wiadomość wyjątku</param>
        public ValidationException(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }
    }
}
