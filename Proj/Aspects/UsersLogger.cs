using mongoDB;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mongoDB.Aspects
{
    /// <summary>
    /// Klasa aspektu logująca zmiany w pliku dla użytkowników.
    /// </summary>
    [PSerializable]
    public class UsersLogger : OnMethodBoundaryAspect
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wykona się poprawnie. Metoda zapisuje do logów informacje o rejestracji nowych użytkowników.
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            if (args.Method != null &&
                args.Method.GetParameters() != null &&
                args.Method.GetParameters().Length > 0 &&
                args.Method.GetParameters()[0].Name.ToLower().Equals("username"))
            {
                var username = (string)args.Arguments.GetArgument(0);

                if (args.Method.Name.Equals("LogIn"))
                {
                    log.Debug(string.Format("User {0} was logged into app", username));
                }
                else if (args.Method.Name.Equals("Register"))
                {
                    log.Info(string.Format("User {0} SUCCESSFULLY registered", username));
                }
            }
            else
            {
                log.Debug($"Method {args.Method.Name} was called");
            }
        }

        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wyrzuci wyjątek. Metoda zapisuje do logów informacje o niepowodzeniu
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnException(MethodExecutionArgs args)
        {
            log.Error($"Exception {args.Exception.GetType().Name} occurred - {args.Exception.Message}");
        }
    }
}
