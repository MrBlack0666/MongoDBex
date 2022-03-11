using mongoDB;
using MongoDB.Bson;
using mongoDB.Exceptions;
using mongoDB.Models;
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
    /// Klasa aspektu logująca zmiany w pliku dla wykonawców.
    /// </summary>
    [PSerializable]
    public class PerformersLogger : OnMethodBoundaryAspect
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();

        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wykona się poprawnie. Metoda zapisuje do logów informacje o dodaniu/edycji/usunięciu wykonawcy.
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            if(args.Method.Name.Equals("AddPerformer"))
                {
                var performer = (Performer)args.Arguments.GetArgument(0);
                log.Info(string.Format("Performer with nickname {0} was SUCCESSFULLY ADDED to database", performer.Nickname));
                return;
            }
            else if (args.Method.Name.Equals("EditPerformer"))
            {
                var performerId = (ObjectId)args.Arguments.GetArgument(0);
                log.Info(string.Format("Performer with id {0} was SUCCESSFULLY EDITED in database", performerId));
                return;
            }
            else if (args.Method.Name.Equals("DeletePerformer"))
            {
                var performerId = (ObjectId)args.Arguments.GetArgument(0);
                log.Info(string.Format("Performer with id {0} was SUCCESSFULLY DELETED from database", performerId));
                return;
            }
        }

        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wyrzuci wyjątek. Metoda zapisuje do logów informacje o niepowodzeniu
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnException(MethodExecutionArgs args)
        {
            var exceptionName = args.Exception.GetType().Name;
            var exceptionMessage = args.Exception.Message;
            if (args.Exception is ValidationException)
            {

                if (args.Method.Name.Equals("AddPerformer"))
                {
                    var performer = (Performer)args.Arguments.GetArgument(0);
                    log.Warn(string.Format("Performer with nickname {0} WASN'T ADDED to database, exception {1} occurred with message: {2}", performer.Nickname, exceptionName, exceptionMessage));
                    return;
                }
                else if (args.Method.Name.Equals("EditPerformer"))
                {
                    var songId = (ObjectId)args.Arguments.GetArgument(0);
                    log.Warn(string.Format("Performer with id {0} WASN'T DELETED from database, exception {1} occurred with message: {2}", songId.ToString(), exceptionName, exceptionMessage));
                    return;
                }
            }

            log.Error($"Undefined exception occurred - {exceptionName}  with message: {args.Exception.Message}");
        }
    }
}
